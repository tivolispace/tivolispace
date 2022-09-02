using System;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public abstract class VoiceMicrophone
    {
        // null is default
        private string _microphoneDeviceName;
        
        private AudioClip _microphone;
        private int _lastPos, _pos;

        public Action<float[]> OnPcmData;

        public void StartMicrophone(bool force = false)
        {
            if (!force && _microphone != null) return;
            Debug.Log("Starting microphone");
            // 5 minutes length
            _microphone = Microphone.Start(_microphoneDeviceName, true, 60 * 5, 44100);
        }
        
        public void StopMicrophone(bool force = false)
        {
            if (!force && _microphone == null) return;
            Debug.Log("Stopping microphone");
            Microphone.End(_microphoneDeviceName);
            _microphone = null;
        }

        public void UpdateMicrophone(string microphoneDeviceName)
        {
            StopMicrophone(true);
            _microphoneDeviceName = microphoneDeviceName;
            StartMicrophone(true);
        }

        public string[] GetMicrophoneNames()
        {
            return Microphone.devices;
        }
        
        public void Update()
        {
            var isRecording = Microphone.IsRecording(_microphoneDeviceName);
            if (!isRecording && _microphone != null) StartMicrophone(true);
            else if (isRecording && _microphone == null) StopMicrophone(true);

            if (_microphone == null) return;
            
            // send voice
            if ((_pos = Microphone.GetPosition(null)) > 0)
            {
                if (_lastPos > _pos)
                {
                    // mic loop reset
                    _lastPos = 0;
                }

                if (_pos - _lastPos > 0)
                {
                    var length = _pos - _lastPos;
                    var samples = new float[length];
                    _microphone.GetData(samples, _lastPos);
                    OnPcmData(samples);
                    _lastPos = _pos;
                }
            }
        }
    }
}