using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    [RequireComponent(typeof(Player.Player))]
    public class PlayerVoiceChat : NetworkBehaviour
    {
        private Player.Player _player;

        private const int TargetSampleRate = Microphone.MicrophoneSampleRate;

        private Microphone _microphone;
        private OpusEncoderThreaded _opusEncoderThreaded;
        private OpusDecoderThreaded _opusDecoderThreaded;

        private bool _resamplingRequired;
        private SpeexMonoResamplerThreaded _resamplerThreaded;
        
        public PlayerVoiceChatOutput playerVoiceChatOutput;
        
        private void Awake()
        {
            _player = GetComponent<Player.Player>();
        }

        public override void OnStartClient()
        {
            // make encoder and decoder sample rates and channel count the same
            if (isLocalPlayer)
            {
                _opusEncoderThreaded = new OpusEncoderThreaded(TargetSampleRate, 1);
                _opusEncoderThreaded.OnEncoded += OnMicrophoneEncoded;

                _microphone = new Microphone
                {
                    OnPcmSamples = OnMicrophonePcmSamples
                };
                _microphone.StartMicrophone();
            }
            else
            {
                _opusDecoderThreaded = new OpusDecoderThreaded(TargetSampleRate, 1);
                _opusDecoderThreaded.OnDecoded += OnVoiceDecoded;
                
                // this is set to 48000 in unity settings, and should stay that way
                // but on macos it'll freely change. therefore we need to resample
                // TODO: resampler doesnt work well from 48000 to 44100
                
                // also, when audio devices change on macos, it seems that it'll
                // magically resample from the old sample rate to the new sample rate.
                // the user only needs to restart if they want to increase the sound
                // quality if they're going from from something low like 24k to 48k 

                var outputSampleRate = AudioSettings.outputSampleRate;
                if (outputSampleRate == TargetSampleRate) return;
                
                Debug.LogWarning(
                    $"Unity can't resample to {TargetSampleRate} so we're manually resampling to system {outputSampleRate}");

                _resamplingRequired = true;
                _resamplerThreaded = new SpeexMonoResamplerThreaded(TargetSampleRate, outputSampleRate);
                _resamplerThreaded.OnResampled += OnResampled;
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

            // FOR TESTING send straight to output
            // playerVoiceChatOutput.AddPcmSamples(pcmSamples);

            // Debug.Log("adding samples: "+pcmSamples.Length);
            _opusEncoderThreaded.AddToEncodeQueue(pcmSamples);
        }

        private void OnMicrophoneEncoded(byte[] opusData)
        {
            // Debug.Log("encoded: "+opusData.Length);
            
            // FOR TESTING send straight to decoder
            // _opusDecoderThreaded.AddToDecodeQueue(opusData);
            
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
            if (_opusDecoderThreaded == null)
            {
                Debug.LogError("Failed to decode voice because decoder isn't available");
                return;
            }

            _opusDecoderThreaded.AddToDecodeQueue(opusData);
        }

        private void OnVoiceDecoded(float[] pcmSamples)
        {
            if (_resamplingRequired)
            {
                _resamplerThreaded.AddToResampleQueue(pcmSamples);
            }
            else
            {
                playerVoiceChatOutput.AddPcmSamples(pcmSamples);
            }
        }

        private void OnResampled(float[] pcmSamples)
        {
            playerVoiceChatOutput.AddPcmSamples(pcmSamples);
        }

        private void Update()
        {
            _microphone?.Update();
            _opusEncoderThreaded?.Update();
            _opusDecoderThreaded?.Update();
            _resamplerThreaded?.Update();
        }
    }
}