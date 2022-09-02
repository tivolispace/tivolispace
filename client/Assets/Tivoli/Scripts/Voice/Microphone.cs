// #define USE_TEST_CLIP

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class Microphone
    {
        private string _microphoneDeviceName;  // null is default mic
        private AudioClip _microphone;

        public Action<float[]> OnPcmSamples;

        // opus doesnt take 44100, but does take 48000 
        public const int MicrophoneSampleRate = 48000;
        // long enough to not hear clipping but short enough to fit in memory
        private const int MicrophoneRecordLength = 60 * 5;
        
        private int _previousPosition;
        private int _totalSamplesSent;
        private int _numTimesLooped;
        
        // 960 samples per outgoing packet, though opus will compress super well
        private const int NumFramesPerOutgoingPacket = 2;
        public const int NumSamplesPerOutgoingPacket = NumFramesPerOutgoingPacket * MicrophoneSampleRate / 100;

        public void StartMicrophone(bool force = false)
        {
            if (!force && _microphone != null) return;

            #if USE_TEST_CLIP
            Debug.Log("Starting microphone with test clip");
            // clip is 2 channels on purpose but same sample rate at 48000
            // actually its forced to mono for now because stereo to mono isn't working
            // TODO: make sure stereo to mono is working
            _microphone = Resources.Load<AudioClip>("dankpods-testing-microphones");
            #else
            Debug.Log("Starting microphone");
            _microphone = UnityEngine.Microphone.Start(_microphoneDeviceName, true, MicrophoneRecordLength, MicrophoneSampleRate);
            #endif

            if (_microphone.frequency != MicrophoneSampleRate)
            {
                Debug.LogError("Selected microphone is not at target sample rate of " + MicrophoneSampleRate);
                StopMicrophone(true);
            }

            if (_microphone.channels > 2)
            {
                Debug.LogError("Selected microphone has more than 2 channels");
                StopMicrophone(true);
            }
        }

        public void StopMicrophone(bool force = false)
        {
            if (!force && _microphone == null) return;
            #if USE_TEST_CLIP
            Debug.Log("Stopping microphone with test clip");
            #else
            Debug.Log("Stopping microphone");
            UnityEngine.Microphone.End(_microphoneDeviceName);
            #endif
            _microphone = null;
        }

        ~Microphone()
        {
            StopMicrophone();
        }

        public void UpdateMicrophone(string microphoneDeviceName)
        {
            StopMicrophone();
            _microphoneDeviceName = microphoneDeviceName;
            StartMicrophone();
        }

        public string[] GetMicrophoneNames()
        {
            return UnityEngine.Microphone.devices;
        }

        private static float[] StereoToMono(IReadOnlyList<float> samples)
        {
            var monoSamples = new float[samples.Count / 2];
            for (var i = 0; i < monoSamples.Length; i++)
            {
                var left = samples[i * 2];
                var right = samples[i * 2 + 1];
                monoSamples[i] = (left + right) / 2f;
            }
            
            return monoSamples;
        }

        public void Update()
        {
            // microphone sometimes turns off when people join
            #if !USE_TEST_CLIP
            var isRecording = UnityEngine.Microphone.IsRecording(_microphoneDeviceName);
            if (!isRecording && _microphone != null) {
                Debug.Log("Microphone turned off for no reason, restarting...");
                StartMicrophone(true);
            }
            #endif

            if (_microphone == null) return;
            
            #if USE_TEST_CLIP
            var currentPosition = (int) (Time.time * _microphone.frequency) % _microphone.samples;
            #else
            var currentPosition = UnityEngine.Microphone.GetPosition(_microphoneDeviceName);
            #endif

            if (currentPosition < _previousPosition)
            {
                _numTimesLooped++;
            }

            var totalSamples = currentPosition + _numTimesLooped * _microphone.samples;
            _previousPosition = currentPosition;
            
            while (totalSamples - _totalSamplesSent >= NumSamplesPerOutgoingPacket * _microphone.channels)
            {
                var samples = new float[NumSamplesPerOutgoingPacket * _microphone.channels];
                _microphone.GetData(samples, _totalSamplesSent % _microphone.samples);
                _totalSamplesSent += NumSamplesPerOutgoingPacket * _microphone.channels;
                
                switch (_microphone.channels)
                {
                    case 1:
                        OnPcmSamples(samples);
                        break;
                    case 2:
                        // will divide length by 2 again since we're doing * channels above
                        OnPcmSamples(StereoToMono(samples));
                        break;
                }
            }
        }
    }
}