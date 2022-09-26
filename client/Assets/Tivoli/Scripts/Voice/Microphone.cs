// #define USE_TEST_CLIP

using System;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class Microphone
    {
        private string _microphoneDeviceName;  // null is default mic
        private AudioClip _microphone;

        public Action<float[]> OnPcmSamples;
        public Action<float, bool> OnInputLevelAndTalking;

        // the whole program should revolve around this sample rate
        public const int MicrophoneSampleRate = 48000;
        // long enough to not hear clipping but short enough to fit in memory
        private const int MicrophoneRecordLength = 60 * 5;
        
        private int _previousPosition;
        private int _totalSamplesSent;
        private int _numTimesLooped;
        
        // 960 samples per outgoing packet, though opus will compress super well
        public const int NumFramesPerOutgoingPacket = 2;
        public const int NumSamplesPerOutgoingPacket = NumFramesPerOutgoingPacket * MicrophoneSampleRate / 100;

        // private SpeexMonoPreprocessor _preprocessor;
        private RnnoiseThreaded _rnnoiseThreaded;

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

            // usually doesn't happen. even on mac with airpods max where the mic is 24000,
            // the audio clip will output 48000. hooray lol no manual resampling
            if (_microphone.frequency != MicrophoneSampleRate)
            {
                Debug.LogError($"Selected microphone has sample rate of {_microphone.frequency} but should be {MicrophoneSampleRate}");
                StopMicrophone(true);
            }

            if (_microphone.channels > 2)
            {
                Debug.LogError("Selected microphone has more than 2 channels");
                StopMicrophone(true);
            }

            // if (_preprocessor == null)
            // {
            //     // https://github.com/mumble-voip/mumble/blob/master/src/mumble/AudioInput.cpp#L743
            //     
            //     _preprocessor = new SpeexMonoPreprocessor(NumSamplesPerOutgoingPacket, MicrophoneSampleRate);
            //
            //     _preprocessor.SetBool(SpeexNative.SpeexPreprocessRequest.SetAgc, true);
            //     _preprocessor.SetFloat(SpeexNative.SpeexPreprocessRequest.SetAgcLevel, MicrophoneSampleRate);
            //     // AgcLevel default is float 8000
            //     // AgcIncrement default is int 12 dB/s
            //     // AgcDecrement default is int -40 dB/s
            //     // AgcMaxGain default is int 30
            //     
            //     _preprocessor.SetBool(SpeexNative.SpeexPreprocessRequest.SetDenoise, true);
            //     // NoiseSuppress default is
            //     
            //     _preprocessor.SetBool(SpeexNative.SpeexPreprocessRequest.SetDereverb, true);
            //     // no settings
            //     
            //     // _preprocessor.SetBool(SpeexNative.SpeexPreprocessRequest.SetEchoSuppressActive, true);
            //     // need a speex_echo_state
            //     // EchoSuppress default is
            //     // EchoSuppressState ??
            // }

            if (_rnnoiseThreaded == null)
            {
                _rnnoiseThreaded = new RnnoiseThreaded(NumSamplesPerOutgoingPacket);
                _rnnoiseThreaded.OnDenoise += OnDenoise;
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
                        InternalOnPcmSamples(samples);
                        break;
                    case 2:
                        // will divide length by 2 again since we're doing * channels above
                        InternalOnPcmSamples(AudioUtils.StereoToMono(samples));
                        break;
                }
            }

            if (_rnnoiseThreaded != null)
            {
                _rnnoiseThreaded.Update();
            }
        }
        
        private const int VoiceHold = 20;
        // private const float VadMin = 0.80f;
        // private const float VadMax = 0.96f;
        private const float VadMin = 0.40f; // stops when under
        private const float VadMax = 0.48f; // starts when over
                    
        private bool _previousVoice;
        private int _holdFrames;
        
        private void InternalOnPcmSamples(float[] monoSamples)
        {
            _rnnoiseThreaded?.AddToDenoiseQueue(monoSamples);
        }

        private void OnDenoise(float[] denoisedSamples, float vadProb)
        {
            // https://github.com/mumble-voip/mumble/blob/master/src/mumble/AudioInput.cpp#L955
            
            // var prob = _preprocessor.GetInt(SpeexNative.SpeexPreprocessRequest.GetProb);
            // var level = prob / 100f;
            
            var talking = vadProb > VadMax || (vadProb > VadMin && _previousVoice);
            
            if (!talking)
            {
                _holdFrames++;
                if (_holdFrames < VoiceHold)
                {
                    talking = true;
                }
            }
            else
            {
                _holdFrames = 0;
            }
            
            _previousVoice = talking;


            if (talking)
            {
                OnPcmSamples(denoisedSamples);
            }
            
            OnInputLevelAndTalking(talking ? AudioUtils.Amplitude(denoisedSamples) * 4f : 0f, talking);
        }
    }
}