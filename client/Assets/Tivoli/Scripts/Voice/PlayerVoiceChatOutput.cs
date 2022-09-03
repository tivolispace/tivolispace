using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerVoiceChatOutput : MonoBehaviour
    {
        private AudioSource _audioSource;
        private AudioClip _dummyClip;

        private readonly PlaybackBuffer _playbackBuffer = new();

        private const int TargetSampleRate = Microphone.MicrophoneSampleRate;

        private SpeexMonoResampler _resampler;
        private bool _resamplingRequired;
        private float _resampleFactor;

        private void Awake()
        {
            _dummyClip = AudioClip.Create("Dummy", TargetSampleRate, 1, TargetSampleRate, false);
            var samples = new float[TargetSampleRate];
            for (var i = 0; i < TargetSampleRate; i++)
            {
                samples[i] = 1f;
            }

            _dummyClip.SetData(samples, 0);
            _dummyClip.hideFlags = HideFlags.DontSave;

            // this is set to 48000 in unity settings, and should stay that way
            // but on macos it'll freely change. therefore we need to resample
            // TODO: resampler doesnt work well from 48000 to 44100
            
            var outputSampleRate = AudioSettings.outputSampleRate;
            if (outputSampleRate != TargetSampleRate)
            {
                _resampler = new SpeexMonoResampler(TargetSampleRate, outputSampleRate);
                _resamplingRequired = true;
                _resampleFactor = (float) TargetSampleRate / outputSampleRate;
                Debug.Log(_resampleFactor);
                Debug.LogWarning(
                    $"Unity can't resample to {TargetSampleRate} so we're manually resampling to {outputSampleRate}");
            }

            _audioSource = GetComponent<AudioSource>();
#if UNITY_EDITOR
            if (_audioSource.playOnAwake) Debug.LogError("Please turn \"Play On Awake\" off");
#endif
            _audioSource.clip = _dummyClip;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        // private static float[] SamplesToFloat(ushort[] compressed)
        // {
        //     var samples = new float[compressed.Length];
        //     for (var i = 0; i < samples.Length; i++)
        //     {
        //         samples[i] = (float) compressed[i] / ushort.MaxValue * 2f - 1f;
        //     }
        //
        //     return samples;
        // }
        //
        // public void AddUshortSamples(ushort[] compressed)
        // {
        //     _playbackBuffer.AddPcmBuffer(SamplesToFloat(compressed));
        // }

        public void AddPcmSamples(float[] pcmSamples)
        {
            _playbackBuffer.AddPcmBuffer(pcmSamples);
        }

        // TODO: there has got to be a way to optimize this
        private static void MonoToStereo(IReadOnlyList<float> mono, IList<float> stereo)
        {
            for (var i = 0; i < mono.Count; i++)
            {
                stereo[i * 2] = mono[i];
                stereo[i * 2 + 1] = mono[i];
            }
        }

        private void OnAudioFilterRead(float[] output, int channels)
        {
            if (channels > 2) return;

            var outputMonoLength = output.Length / channels;
            // Debug.Log("outputMonoLength: "+ outputMonoLength);

            var lengthWeNeed = _resamplingRequired
                ? Mathf.CeilToInt(outputMonoLength * _resampleFactor)
                : outputMonoLength;
            // Debug.Log("lengthWeNeed: "+lengthWeNeed);

            // we dont have enough samples to play yet
            if (_playbackBuffer.GetAvailableSamples() < lengthWeNeed * 2) return;

            var bufferRawData = new float[lengthWeNeed];
            var bufferRead = _playbackBuffer.Read(bufferRawData, 0, bufferRawData.Length);
            var bufferData = new float[bufferRead];
            Array.Copy(bufferRawData, bufferData, bufferRead);
            // Debug.Log("bufferRead: " +bufferRead);

            if (_resamplingRequired)
            {
                var resampledData = new float[outputMonoLength];
                var resampledRead = _resampler.Resample(bufferData, resampledData);
                // Debug.Log("resampledRead: " +resampledRead);

                if (channels == 2)
                {
                    MonoToStereo(resampledData, output);
                }
                else
                {
                    Array.Copy(resampledData, output, output.Length);
                }
            }
            else
            {
                if (channels == 2)
                {
                    MonoToStereo(bufferData, output);
                }
                else
                {
                    Array.Copy(bufferData, output, output.Length);
                }
            }
        }

        private void Update()
        {
            if (_audioSource.isPlaying == false)
            {
                Debug.LogWarning("Audio source randomly stopped playing, starting again");
                _audioSource.Play();
            }
        }
    }
}