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

        private void Awake()
        {
            _dummyClip = AudioClip.Create("Dummy", TargetSampleRate, 1, TargetSampleRate, false);
            _dummyClip.hideFlags = HideFlags.DontSave;

            _audioSource = GetComponent<AudioSource>();
#if UNITY_EDITOR
            if (_audioSource.playOnAwake) Debug.LogError("Please turn \"Play On Awake\" off");
#endif
            _audioSource.clip = _dummyClip;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        public void AddPcmSamples(float[] pcmSamples)
        {
            _playbackBuffer.AddPcmBuffer(pcmSamples);
        }
        
        // TODO: there has got to be a way to optimize this
        private static void MonoToStereo(IReadOnlyList<float> mono, float[] stereo)
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
            
            // we dont have enough samples to play yet
            if (_playbackBuffer.GetAvailableSamples() < output.Length / channels) return;

            if (channels == 1)
            {
                _playbackBuffer.Read(output, 0, output.Length);
            }
            else
            {
                var mono = new float[output.Length / channels];
                _playbackBuffer.Read(mono, 0, mono.Length);
                MonoToStereo(mono, output);
            }
        }

        private void Update()
        {
            if (_audioSource.isPlaying == false)
            {
                // Debug.Log("Audio source randomly stopped playing, starting again");
                _audioSource.Play();
            }
        }
    }
}