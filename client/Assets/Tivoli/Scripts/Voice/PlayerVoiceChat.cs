using Mirror;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    [RequireComponent(typeof(Player.Player))]
    public class PlayerVoiceChat : NetworkBehaviour
    {
        private Player.Player _player;

        private OpusEncoderThreaded _opusEncoderThreaded;
        private Microphone _microphone;
        
        private OpusDecoderThreaded _opusDecoderThreaded;
        public PlayerVoiceChatOutput playerVoiceChatOutput;
        
        private int _sent, _recv;

        private void Awake()
        {
            _player = GetComponent<Player.Player>();
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                _opusEncoderThreaded = new OpusEncoderThreaded(Microphone.MicrophoneSampleRate, 1);
                _opusEncoderThreaded.OnEncoded += OnMicrophoneEncoded;
                
                _microphone = new Microphone();
                _microphone.StartMicrophone();
                _microphone.OnPcmSamples = OnMicrophonePcmSamples;
            }
            else
            {
                // TODO: not all machines have 48000 sample rate
                _opusDecoderThreaded = new OpusDecoderThreaded(Microphone.MicrophoneSampleRate, 2);
                _opusDecoderThreaded.OnDecoded += OnVoiceDecoded;
            }
        }

        public override void OnStopClient()
        {
            if (isLocalPlayer)
            {
                _microphone = null;
                _opusEncoderThreaded = null;
            }
            else
            { 
                _opusDecoderThreaded = null;
            }
        }
        
        // pipeline starts here and goes down fn by fn

        private void OnMicrophonePcmSamples(float[] pcmSamples)
        {
            // var ushortSamples = SamplesToUshort(pcmSamples);
            // CmdSendVoice(ushortSamples);
            // Transport.activeTransport.GetMaxPacketSize(Channels.Unreliable);

            if (_opusEncoderThreaded == null)
            {
                Debug.LogError("Failed to encode voice because encoder isn't available");
                return;
            }
            
            // Debug.Log("adding samples: "+pcmSamples.Length);
            _opusEncoderThreaded.AddToEncodeQueue(pcmSamples);
        }

        private void OnMicrophoneEncoded(byte[] opusData)
        {
            _sent += opusData.Length;
            // Debug.Log("encoded: "+opusData.Length);
            CmdSendVoice(opusData);
        }
        
        [Command(channel = Channels.Unreliable, requiresAuthority = true)]
        private void CmdSendVoice(byte[] opusData)
        {
            RpcReceiveVoice(opusData);
        }
        
        [ClientRpc(channel = Channels.Unreliable, includeOwner = false)]
        private void RpcReceiveVoice(byte[] opusData)
        {
            _recv += opusData.Length;
            
            if (_opusDecoderThreaded == null)
            {
                Debug.LogError("Failed to decoded voice because decoder isn't available");
                return;
            }
        
            _opusDecoderThreaded.AddToDecodeQueue(opusData);
        }
        
        private void OnVoiceDecoded(float[] pcmSamples)
        {
            playerVoiceChatOutput.AddPcmSamples(pcmSamples);
        }

        private void Update()
        {
            _microphone?.Update();
            _opusEncoderThreaded?.Update();
            _opusDecoderThreaded?.Update();
        }
        
        private void OnGUI()
        {
            if (isLocalPlayer)
            {
                GUI.TextArea(new Rect(0, 100, 400, 20), "Opus sent: " + _sent);
                GUI.TextArea(new Rect(0, 120, 400, 20), "Opus recv: " + _recv);
            }
            else
            {
                GUI.TextArea(new Rect(0, 180, 400, 20), "Other opus sent: " + _sent);
                GUI.TextArea(new Rect(0, 200, 400, 20), "Other opus recv: " + _recv);
            }
        }
    }
}