using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerVoiceChatOutput : MonoBehaviour
    {
        private AudioSource _audioSource;

        private readonly PlaybackBuffer _playbackBuffer = new();

        private const int InputSampleRate = 48000;
        private const int OutputSampleRate = 44100;
        private const float ResampleFactor = (float) InputSampleRate / OutputSampleRate;

        // private bool _resamplingRequired = true;
        private readonly SpeexMonoResampler _resampler = new(InputSampleRate, OutputSampleRate);
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
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

        private static void MonoToStereo(float[] mono, float[] stereo)
        {
            for (var i = 0; i < mono.Length; i++)
            {
                stereo[i * 2] = mono[i];
                stereo[i * 2 + 1] = mono[i];
            }
        }

        private void OnAudioFilterRead(float[] output, int channels)
        {
            // if (channels > 2) return;
            //
            // Debug.Log("ON READ");
            //
            // var outputMonoLength = output.Length / channels;
            //
            // var lengthWeNeedBeforeResample = Mathf.CeilToInt(outputMonoLength * ResampleFactor);
            // if (_playbackBuffer.GetAvailableSamples() < lengthWeNeedBeforeResample * 2)
            // {
            //     return;
            // }
            //
            // Debug.Log("to fill"+output.Length);
            // Debug.Log("we need"+lengthWeNeedBeforeResample);
            //
            // Debug.Log("buffer has:"+_playbackBuffer.GetAvailableSamples());
            //
            // var bufferRawData = new float[lengthWeNeedBeforeResample];
            // var bufferRead = _playbackBuffer.Read(bufferRawData, 0, bufferRawData.Length);
            // Debug.Log("buffer asked:" +lengthWeNeedBeforeResample);
            // Debug.Log("buffer read:"+bufferRead);
            // var bufferData = new float[bufferRead];
            // Array.Copy(bufferRawData, bufferData, bufferRead);
            //
            // var resampledData = new float[outputMonoLength];
            // var resampledRead = _resampler.Resample(bufferData, resampledData);
            // Debug.Log("resampled asked:" +bufferData.Length);
            // Debug.Log("resampled read:" +resampledRead);
            //
            // if (channels == 2)
            // {
            //     MonoToStereo(resampledData, output);
            // }
            // else
            // {
            //     Array.Copy(resampledData, output, resampledData.Length);
            // }

            if (channels > 2) return;
            
            var outputMonoLength = output.Length / channels;
            
            // * 2 just to give some slack for now
            if (_playbackBuffer.GetAvailableSamples() < outputMonoLength)
            {
                return;
            }
            
            Debug.Log("to fill"+output.Length);
            Debug.Log("we need"+outputMonoLength);
            
            Debug.Log("buffer has:"+_playbackBuffer.GetAvailableSamples());
            
            var bufferRawData = new float[outputMonoLength];
            var bufferRead = _playbackBuffer.Read(bufferRawData, 0, bufferRawData.Length);
            Debug.Log("buffer asked:" +outputMonoLength);
            Debug.Log("buffer read:"+bufferRead);
            var bufferData = new float[bufferRead];
            Array.Copy(bufferRawData, bufferData, bufferRead);

            if (channels == 2)
            {
                MonoToStereo(bufferData, output);
            }
            // else
            // {
            //     Array.Copy(resampledData, output, resampledData.Length);
            // }
        }

        private void Update()
        {
            if (_audioSource.isPlaying == false)
            {
                _audioSource.Play();
            }
        }
    }
}