using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class OpusDecoderThreaded
    {
        private readonly OpusDecoder _opusDecoder;

        private readonly Queue<byte[]> _decoderInput = new();
        private readonly object _decoderInputLock = new();

        private readonly Queue<float[]> _decoderOutput = new();
        private readonly object _decoderOutputLock = new();

        private bool _isRunning = true;
        private readonly Thread _decodeThread;

        public Action<float[]> OnDecoded;

        private int _outputSampleRate;
        private int _outputChannels;

        private const int MaxInQueue = 20;

        public OpusDecoderThreaded(int outputSampleRate, int outputChannels)
        {
            _opusDecoder = new OpusDecoder(outputSampleRate, outputChannels);
            _outputSampleRate = outputSampleRate;
            _outputChannels = outputChannels;
            _decodeThread = new Thread(DecodeThread);
            _decodeThread.Start();
        }

        public void ResetState()
        {
            _opusDecoder.ResetState();
        }

        public void OnDestroy()
        {
            _isRunning = false;
            _decodeThread.Join();
        }

        ~OpusDecoderThreaded()
        {
            OnDestroy();
        }

        public void AddToDecodeQueue(byte[] opusData)
        {
            lock (_decoderInputLock)
            {
                if (_decoderInput.Count > MaxInQueue)
                {
                    Debug.Log($"Decoder has more than {MaxInQueue} inputs waiting, will clear");
                    _decoderInput.Clear();
                }

                _decoderInput.Enqueue(opusData);
            }
        }

        private void DecodeThread()
        {
            while (_isRunning)
            {
                byte[] input;
                lock (_decoderInputLock)
                {
                    if (_decoderInput.Count == 0) continue;
                    input = _decoderInput.Dequeue();
                }

                var pcmRawData =
                    new float[Microphone.NumFramesPerOutgoingPacket * _outputSampleRate / 100 * _outputChannels];
                var length = _opusDecoder.Decode(input, pcmRawData);
                var pcmData = new float[length];
                Array.Copy(pcmRawData, pcmData, length);

                lock (_decoderOutputLock)
                {
                    if (_decoderOutput.Count > MaxInQueue)
                    {
                        Debug.Log($"Decoder has more than {MaxInQueue} outputs waiting, will clear");
                        _decoderOutput.Clear();
                    }

                    _decoderOutput.Enqueue(pcmData);
                }
            }
        }

        public void Update()
        {
            lock (_decoderOutputLock)
            {
                while (_decoderOutput.Count > 0)
                {
                    OnDecoded(_decoderOutput.Dequeue());
                }
            }
        }
    }
}