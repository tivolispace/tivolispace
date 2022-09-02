using System;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerVoiceChatOutput : MonoBehaviour
    {
        private AudioSource _audioSource;

        private Action<float[], float> OnAudioSample;
        private readonly PlaybackBuffer _playbackBuffer = new();

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

        private void OnAudioFilterRead(float[] stereoData, int channels)
        {
            // var monoData = new float[stereoData.Length / 2];
            //
            // var numRead = _playbackBuffer.Read(monoData, 0, monoData.Length);
            // var percentUnderRun = 1f - (float) numRead / monoData.Length;
            //
            // for (var i = 0; i < monoData.Length; i++)
            // {
            //     stereoData[i * 2] = monoData[i];
            //     stereoData[i * 2 + 1] = monoData[i];
            // }
            
            // opus can and does convert to stereo for us so less work is done in this thread
            
            var numRead = _playbackBuffer.Read(stereoData, 0, stereoData.Length);
            var percentUnderRun = 1f - (float) numRead / stereoData.Length;

            OnAudioSample?.Invoke(stereoData, percentUnderRun);
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