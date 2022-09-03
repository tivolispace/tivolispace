using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class OpusEncoderThreaded
    {
        private readonly OpusEncoder _opusEncoder;

        private readonly Queue<float[]> _encoderInput = new();
        private readonly Queue<byte[]> _encoderOutput = new();

        private bool _isRunning = true;
        private readonly Thread _encodeThread;

        public Action<byte[]> OnEncoded;
        
        private const int MaxInQueue = 20;

        public OpusEncoderThreaded(int inputSampleRate, int inputChannels)
        {
            _opusEncoder = new OpusEncoder(inputSampleRate, inputChannels);
            _encodeThread = new Thread(EncodeThread);
            _encodeThread.Start();
        }

        ~OpusEncoderThreaded()
        {
            _isRunning = false;
            _encodeThread.Join();
        }

        public void AddToEncodeQueue(float[] pcmSamples)
        {
            lock (_encoderInput)
            {
                if (_encoderInput.Count > MaxInQueue)
                {
                    Debug.LogWarning($"Encoder has more than {MaxInQueue} inputs waiting, will clear");
                    _encoderInput.Clear();
                }
                _encoderInput.Enqueue(pcmSamples);
            }
        }

        private void EncodeThread()
        {
            while (_isRunning)
            {
                float[] input;
                lock (_encoderInput)
                {
                    if (_encoderInput.Count == 0) continue;
                    input = _encoderInput.Dequeue();
                }

                var output = _opusEncoder.Encode(input);
                lock (_encoderOutput)
                {
                    if (_encoderOutput.Count > MaxInQueue)
                    {
                        Debug.LogWarning($"Encoder has more than {MaxInQueue} outputs waiting, will clear");
                        _encoderOutput.Clear();
                    }
                    _encoderOutput.Enqueue(output);
                }
            }
        }

        public void Update()
        {
            lock (_encoderOutput)
            {
                while (_encoderOutput.Count > 0)
                {
                    OnEncoded(_encoderOutput.Dequeue());
                }
            }
        }
    }
}