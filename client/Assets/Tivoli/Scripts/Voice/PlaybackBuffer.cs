using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tivoli.Scripts.Voice
{
    // should this be replaced with a ring buffer?
    // is this a ring buffer?
    // does it need a better name?
    
    public class PlaybackBuffer
    {
        private struct PcmBuffer
        {
            
            public float[] PcmData;
            public int PcmLength;
            public int ReadOffset;
        }
        
        private int _readCount;
        private PcmBuffer _currentPcmBuffer;
        
        private readonly object _bufferLock = new();
        private readonly Queue<PcmBuffer> _pcmBuffers = new();

        private const int MaxBuffers = 10;

        // TODO: make this better
        public int GetAvailableSamples()
        {
            lock (_bufferLock)
            {
                return _pcmBuffers.ToArray().Sum(pcmBuffer => pcmBuffer.PcmLength);
            }
        }
        
        public int Read(float[] buffer, int offset, int count)
        {
            var readCount = 0;
            while (readCount < count && _readCount > 0)
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
            var samples = _currentPcmBuffer.PcmLength - _currentPcmBuffer.ReadOffset;
            if (samples == 0)
            {
                lock (_bufferLock)
                {
                    if (_pcmBuffers.Count == 0)
                    {
                        // Debug.LogError("No available decode buffers!");
                        return 0;
                    }
                    
                    _currentPcmBuffer = _pcmBuffers.Dequeue();
                    
                    samples = _currentPcmBuffer.PcmLength - _currentPcmBuffer.ReadOffset;
                }
            }
            
            var readCount = Math.Min(samples, count);
            Array.Copy(_currentPcmBuffer.PcmData, _currentPcmBuffer.ReadOffset, dst, dstOffset, readCount);

            Interlocked.Add(ref _readCount, -readCount);
            _currentPcmBuffer.ReadOffset += readCount;
            return readCount;
        }
        
        public void AddPcmBuffer(float[] pcmData)
        {
            var pcmBuffer = new PcmBuffer()
            {
                PcmData = pcmData,
                PcmLength = pcmData.Length,
                ReadOffset = 0
            };

            lock (_bufferLock)
            {
                var count = _pcmBuffers.Count;
                if (count > MaxBuffers)
                {
                    // Debug.LogWarning("Max buffer size reached, dropping");
                }
                else
                {
                    _pcmBuffers.Enqueue(pcmBuffer);
                    Interlocked.Add(ref _readCount, pcmData.Length);
                }
            }
        }
    }
}