using System;
using UnityEngine;

namespace Tivoli.Local_Scripts
{
    public class VoiceMicrophone : MonoBehaviour
    {
        private AudioClip _microphone;
        private int _lastPos, _pos;

        public Action<float[]> OnPcmData;

        public void StartMicrophone(bool force = false)
        {
            if (!force && _microphone != null) return;
            Debug.Log("Starting microphone");
            _microphone = Microphone.Start(null, true, 5, 44100);
        }
        
        public void StopMicrophone(bool force = false)
        {
            if (!force && _microphone == null) return;
            Debug.Log("Stopping microphone");
            Microphone.End(null);
            _microphone = null;
        }
        
        private void Update()
        {
            var isRecording = Microphone.IsRecording(null);
            if (!isRecording && _microphone != null) StartMicrophone(true);
            else if (isRecording && _microphone == null) StopMicrophone(true);
            
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
                    _lastPos = _pos;
                    OnPcmData(samples);
                }
            }
        }
    }
}