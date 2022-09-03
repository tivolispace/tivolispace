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
        private readonly Queue<float[]> _decoderOutput = new();

        private bool _isRunning = true;
        private readonly Thread _decodeThread;

        public Action<float[]> OnDecoded;

        private int _outputSampleRate;
        private int _outputChannels;

        public OpusDecoderThreaded(int outputSampleRate, int outputChannels)
        {
            _opusDecoder = new OpusDecoder(outputSampleRate, outputChannels);
            _outputSampleRate = outputSampleRate;
            _outputChannels = outputChannels;
            _decodeThread = new Thread(DecodeThread);
            _decodeThread.Start();
        }

        ~OpusDecoderThreaded()
        {
            _isRunning = false;
            _decodeThread.Join();
        }

        public void AddToDecodeQueue(byte[] opusData)
        {
            lock (_decoderInput)
            {
                if (_decoderInput.Count > 50)
                {
                    Debug.LogWarning("Decoder has more than 50 inputs waiting, will clear");
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
                lock (_decoderInput)
                {
                    if (_decoderInput.Count == 0) continue;
                    input = _decoderInput.Dequeue();
                }

                var pcmData = new float[Microphone.NumFramesPerOutgoingPacket * _outputSampleRate / 100 *
                                        _outputChannels];

                var length = _opusDecoder.Decode(input, pcmData);

                lock (_decoderOutput)
                {
                    if (_decoderOutput.Count > 50)
                    {
                        Debug.LogWarning("Decoder has more than 50 outputs waiting, will clear");
                        _decoderOutput.Clear();
                    }

                    _decoderOutput.Enqueue(pcmData);
                }
            }
        }

        public void Update()
        {
            lock (_decoderOutput)
            {
                while (_decoderOutput.Count > 0)
                {
                    OnDecoded(_decoderOutput.Dequeue());
                }
            }
        }
    }
}