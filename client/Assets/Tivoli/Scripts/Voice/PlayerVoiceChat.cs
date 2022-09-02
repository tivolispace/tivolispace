using Mirror;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    [RequireComponent(typeof(Player.Player))]
    public class PlayerVoiceChat : NetworkBehaviour
    {
        private Player.Player _player;

        private readonly Microphone _microphone = new();

        private int _sent, _recv;

        public PlayerVoiceChatOutput playerVoiceChatOutput;

        private void Awake()
        {
            _player = GetComponent<Player.Player>();
        }

        public override void OnStartLocalPlayer()
        {
           _microphone.StartMicrophone();
           _microphone.OnPcmData = SendPcmSamples;
        }

        public override void OnStopLocalPlayer()
        {
            _microphone.StopMicrophone();
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

        private void Update()
        {
            _microphone.Update();
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