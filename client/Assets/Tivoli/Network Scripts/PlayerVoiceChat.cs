using Mirror;
using Tivoli.Local_Scripts;
using UnityEngine;

namespace Tivoli.Network_Scripts
{
    public class PlayerVoiceChat : NetworkBehaviour
    {
        private Player _player;

        private VoiceMicrophone _voiceMicrophone;

        private int _sent, _recv;

        public PlayerVoiceChatOutput playerVoiceChatOutput;

        private void Awake()
        {
            _player = GetComponent<Player>();

            AudioSettings.outputSampleRate = 44100;
            AudioSettings.speakerMode = AudioSpeakerMode.Stereo;
        }

        public override void OnStartLocalPlayer()
        {
            var voiceMicrophone = new GameObject
            {
                name = "Voice Microphone"
            };
            _voiceMicrophone = voiceMicrophone.AddComponent<VoiceMicrophone>();

            _voiceMicrophone.OnPcmData += SendPcmSamples;
            
            _voiceMicrophone.StartMicrophone();
        }

        public override void OnStopLocalPlayer()
        {
            _voiceMicrophone.StopMicrophone();
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