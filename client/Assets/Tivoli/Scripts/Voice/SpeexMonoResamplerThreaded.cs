using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class SpeexMonoResamplerThreaded
    {
        private readonly SpeexMonoResampler _resampler;

        private readonly Queue<float[]> _resampleInput = new();
        private readonly object _resampleInputLock = new();

        private readonly Queue<float[]> _resampleOutput = new();
        private readonly object _resampleOutputLock = new();

        private bool _isRunning = true;
        private readonly Thread _resampleThread;

        public Action<float[]> OnResampled;

        private const int MaxInQueue = 20;

        private readonly float _resampleFactor;

        public SpeexMonoResamplerThreaded(int inputSampleRate, int outputSampleRate)
        {
            _resampleFactor = (float) inputSampleRate / outputSampleRate;
            _resampler = new SpeexMonoResampler(inputSampleRate, outputSampleRate);
            _resampleThread = new Thread(ResampleThread);
            _resampleThread.Start();
        }

        public void OnDestroy()
        {
            _isRunning = false;
            _resampleThread.Join();
            _resampler.OnDestroy();
        }

        ~SpeexMonoResamplerThreaded()
        {
            OnDestroy();
        }

        public void AddToResampleQueue(float[] pcmSamples)
        {
            lock (_resampleInputLock)
            {
                if (_resampleInput.Count > MaxInQueue)
                {
                    Debug.Log($"Resample has more than {MaxInQueue} inputs waiting, will clear");
                    _resampleInput.Clear();
                }

                _resampleInput.Enqueue(pcmSamples);
            }
        }

        private void ResampleThread()
        {
            while (_isRunning)
            {
                float[] input;
                lock (_resampleInputLock)
                {
                    if (_resampleInput.Count == 0) continue;
                    input = _resampleInput.Dequeue();
                }

                var resampledRawData = new float[Mathf.CeilToInt(input.Length * _resampleFactor)];
                var length = _resampler.Resample(input, resampledRawData);
                var resampledData = new float[length];
                Array.Copy(resampledRawData, resampledData, length);

                lock (_resampleOutputLock)
                {
                    if (_resampleOutput.Count > MaxInQueue)
                    {
                        Debug.Log($"Resampler has more than {MaxInQueue} outputs waiting, will clear");
                        _resampleOutput.Clear();
                    }

                    _resampleOutput.Enqueue(resampledData);
                }
            }
        }

        public void Update()
        {
            lock (_resampleOutputLock)
            {
                while (_resampleOutput.Count > 0)
                {
                    OnResampled(_resampleOutput.Dequeue());
                }
            }
        }
    }
}