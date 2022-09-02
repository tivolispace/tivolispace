using System;
using System.Collections.Generic;
using System.Threading;

namespace Tivoli.Scripts.Voice
{
    public class VoicePlaybackBuffer
    {
        private struct Decoded
        {
            
            public float[] PcmData;
            public int PcmLength;
            public int ReadOffset;
        }
        
        private int _decodedCount;
        private Decoded _currentPacket;
        
        private readonly object _bufferLock = new();
        private readonly Queue<Decoded> _decodedBuffers = new();

        public int Read(float[] buffer, int offset, int count)
        {
            var readCount = 0;
            while (readCount < count && _decodedCount > 0)
            {
                readCount += ReadFromBuffer(buffer, offset + readCount, count - readCount);
            }

            if (readCount == 0)
            {
                Array.Clear(buffer, offset, count);
            }
            else if (readCount < count)
            {
                Array.Clear(buffer, offset + readCount, count - readCount);
            }

            return readCount;
        }
        
        private int ReadFromBuffer(float[] dst, int dstOffset, int count)
        {
            var samples = _currentPacket.PcmLength - _currentPacket.ReadOffset;
            if (samples == 0)
            {
                lock (_bufferLock)
                {
                    if (_decodedBuffers.Count == 0)
                    {
                        // Debug.LogError("No available decode buffers!");
                        return 0;
                    }
                    
                    _currentPacket = _decodedBuffers.Dequeue();
                    
                    samples = _currentPacket.PcmLength - _currentPacket.ReadOffset;
                }
            }
            
            var readCount = Math.Min(samples, count);
            Array.Copy(_currentPacket.PcmData, _currentPacket.ReadOffset, dst, dstOffset, readCount);

            Interlocked.Add(ref _decodedCount, -readCount);
            _currentPacket.ReadOffset += readCount;
            return readCount;
        }
        
        public void AddDecodedAudio(float[] pcmData)
        {
            var decoded = new Decoded()
            {
                PcmData = pcmData,
                PcmLength = pcmData.Length,
                ReadOffset = 0
            };

            lock (_bufferLock)
            {
                var count = _decodedBuffers.Count;
                if (count > 10) // const
                {
                    // Debug.LogWarning("Max buffer size reached, dropping");
                }
                else
                {
                    _decodedBuffers.Enqueue(decoded);
                    Interlocked.Add(ref _decodedCount, pcmData.Length);
                }
            }
        }
    }
}