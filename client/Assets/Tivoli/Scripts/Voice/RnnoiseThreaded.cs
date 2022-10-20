using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class RnnoiseThreaded
    {
        private readonly Rnnoise _rnnoise;
        private readonly int _denoiseFrameSize;
        private readonly int _outputFrameSize;

        private readonly Queue<float[]> _denoiseInput = new();
        private readonly object _denoiseInputLock = new();

        private readonly PlaybackBuffer _denoiseInputBuffer = new();

        private readonly Queue<(float[], float)> _denoiseOutput = new();
        private readonly object _denoiseOutputLock = new();

        private readonly PlaybackBuffer _denoiseOutputBuffer = new();
        private readonly PlaybackBuffer _denoiseOutputBufferVadProb = new();

        private bool _isRunning = true;
        private readonly Thread _denoiseThread;

        public Action<float[], float> OnDenoise;

        private const int MaxInQueue = 20;

        public RnnoiseThreaded(int outputFrameSize)
        {
            _rnnoise = new Rnnoise();
            _denoiseFrameSize = _rnnoise.FrameSize;
            _outputFrameSize = outputFrameSize;
            _denoiseThread = new Thread(DenoiseThread);
            _denoiseThread.Start();
        }

        public void OnDestroy()
        {
            _isRunning = false;
            _denoiseThread.Join();
            _rnnoise.OnDestroy();
        }

        ~RnnoiseThreaded()
        {
            OnDestroy();
        }

        public void AddToDenoiseQueue(float[] pcmSamples)
        {
            _denoiseInputBuffer.AddPcmBuffer(pcmSamples);
        }

        private void DenoiseThread()
        {
            while (_isRunning)
            {
                float[] input;
                lock (_denoiseInputLock)
                {
                    if (_denoiseInput.Count == 0) continue;
                    input = _denoiseInput.Dequeue();
                }

                var denoiseData = new float[input.Length];
                var vadProb = _rnnoise.Process(input, denoiseData);

                lock (_denoiseOutputLock)
                {
                    if (_denoiseOutput.Count > MaxInQueue)
                    {
                        Debug.Log($"Denoiser has more than {MaxInQueue} outputs waiting, will clear");
                        _denoiseOutput.Clear();
                    }

                    _denoiseOutput.Enqueue((denoiseData, vadProb));
                }
            }
        }

        public void Update()
        {
            if (_denoiseInputBuffer.GetAvailableSamples() >= _denoiseFrameSize)
            {
                var frameSizePcmSamples = new float[_denoiseFrameSize];
                _denoiseInputBuffer.Read(frameSizePcmSamples, 0, _denoiseFrameSize);

                lock (_denoiseInputLock)
                {
                    if (_denoiseInput.Count > MaxInQueue)
                    {
                        Debug.Log($"Denoiser has more than {MaxInQueue} inputs waiting, will clear");
                        _denoiseInput.Clear();
                    }

                    _denoiseInput.Enqueue(frameSizePcmSamples);
                }
            }

            lock (_denoiseOutputLock)
            {
                while (_denoiseOutput.Count > 0)
                {
                    var (denoiseData, vadProb) = _denoiseOutput.Dequeue();
                    _denoiseOutputBuffer.AddPcmBuffer(denoiseData);

                    // same length as pcm data but only vad prob
                    var vadProbData = new float[denoiseData.Length];
                    for (var i = 0; i < vadProbData.Length; i++)
                    {
                        vadProbData[i] = vadProb;
                    }

                    _denoiseOutputBufferVadProb.AddPcmBuffer(vadProbData);
                }
            }

            if (_denoiseOutputBuffer.GetAvailableSamples() >= _outputFrameSize)
            {
                var outputData = new float[_outputFrameSize];
                _denoiseOutputBuffer.Read(outputData, 0, _outputFrameSize);

                // try to guess vad prob as much as possible
                var outputDataVadProb = new float[_outputFrameSize];
                _denoiseOutputBufferVadProb.Read(outputDataVadProb, 0, _outputFrameSize);
                var vadProbAvg = 0f;
                for (var i = 0; i < _outputFrameSize; i++)
                {
                    vadProbAvg += outputDataVadProb[i];
                }

                vadProbAvg /= _outputFrameSize;

                OnDenoise(outputData, vadProbAvg);
            }
        }
    }
}