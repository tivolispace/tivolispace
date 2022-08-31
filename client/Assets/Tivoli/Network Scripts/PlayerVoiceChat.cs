using System;
using Mirror;
using Tivoli.Local_Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tivoli.Network_Scripts
{
    public class PlayerVoiceChat : NetworkBehaviour
    {
        private Player _player;

        private AudioClip _microphone;
        private int _lastPos, _pos;
        
        private int _sent, _recv;

        public PlayerVoiceChatOutput playerVoiceChatOutput;

        private void Awake()
        {
            _player = GetComponent<Player>();

            AudioSettings.outputSampleRate = 48000;
            AudioSettings.speakerMode = AudioSpeakerMode.Stereo;
        }

        private void StartMicrophone()
        {
            if (_microphone != null) return;
            _microphone = Microphone.Start(null, true, 5, 48000);
        }
        
        private void StopMicrophone()
        {
            if (_microphone == null) return;
            Microphone.End(null);
            _microphone = null;
        }

        public override void OnStartLocalPlayer()
        {
            StartMicrophone();
        }

        public override void OnStopLocalPlayer()
        {
            StopMicrophone();
        }

        // TODO: switch to unreliable when we have opus
        
        [ClientRpc(channel = Channels.Reliable, includeOwner = false)]
        private void RpcReceiveVoice(ushort[] samples)
        {
            _recv += samples.Length;
            playerVoiceChatOutput.AddUshortSamples(samples);
        }
        
        [Command(channel = Channels.Reliable, requiresAuthority = true)]
        private void CmdSendVoice(ushort[] data, NetworkConnectionToClient sender = null)
        {
            RpcReceiveVoice(data);
        }

        private void SendPcmSamples(float[] pcmSamples)
        {
            var ushortSamples = SamplesToUshort(pcmSamples);
            // Transport.activeTransport.GetMaxPacketSize(Channels.Unreliable);
            CmdSendVoice(ushortSamples);
            _sent += pcmSamples.Length;
        }

        private static ushort[] SamplesToUshort(float[] samples)
        {
            var compressed = new ushort[samples.Length];
            for (var i = 0; i < samples.Length; i++)
            {
                compressed[i] = (ushort) ((samples[i] + 1f) / 2f * ushort.MaxValue);
            }

            return compressed;
        }
        
        private float[] GetMicrophoneSamples()
        {
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
                    return samples;
                }
            }

            return new float[] { };
        }

        private void Update()
        {
            if (!isLocalPlayer || !NetworkClient.isConnected) return;

            // send voice
            var samples = GetMicrophoneSamples();
            if (samples.Length > 0)
            {
                SendPcmSamples(samples);
            }
        }

        private void OnGUI()
        {
            if (isLocalPlayer)
            {
                GUI.TextArea(new Rect(0, 100, 400, 20), "Samples sent: " + _sent);
                GUI.TextArea(new Rect(0, 120, 400, 20), "Samples recv: " + _recv);
            }
            else
            {
                GUI.TextArea(new Rect(0, 180, 400, 20), "Other samples sent: " + _sent);
                GUI.TextArea(new Rect(0, 200, 400, 20), "Other samples recv: " + _recv);
            }
        }
    }
}