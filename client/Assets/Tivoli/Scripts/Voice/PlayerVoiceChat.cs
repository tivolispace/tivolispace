// #define SELF_TEST_VOICE

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
#if !SELF_TEST_VOICE
            // make encoder and decoder sample rates and channel count the same
            if (isLocalPlayer)
            {
#endif
                _opusEncoderThreaded = new OpusEncoderThreaded(TargetSampleRate, 1);
                _opusEncoderThreaded.OnEncoded += OnMicrophoneEncoded;

                _microphone = new Microphone
                {
                    OnPcmSamples = OnMicrophonePcmSamples,
                    OnInputLevelAndTalking = OnMicrophoneLevelAndTalking,
                };
                _microphone.StartMicrophone();
#if !SELF_TEST_VOICE
            }
            else
            {
#endif
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
#if !SELF_TEST_VOICE
            }
#endif
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

        private float micLevel;
        private bool micTalking;
        private bool micNeedsResetAfterTalking; 

        private void OnMicrophoneLevelAndTalking(float level, bool talking)
        {
            if (talking == false && micNeedsResetAfterTalking)
            {
                micNeedsResetAfterTalking = false;
                _opusEncoderThreaded.ResetState();
                CmdResetState(); // send as packet to decoders
            }
            micLevel = level;
            micTalking = talking;
        }
        
        private void OnMicrophonePcmSamples(float[] pcmSamples)
        {
            if (_opusEncoderThreaded == null)
            {
                Debug.LogError("Failed to encode voice because encoder isn't available");
                return;
            }

            // FOR TESTING send straight to output
            // playerVoiceChatOutput.AddPcmSamples(pcmSamples);

            // Debug.Log("adding samples: "+pcmSamples.Length);
            _opusEncoderThreaded.AddToEncodeQueue(pcmSamples);
            micNeedsResetAfterTalking = true;
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
        
                
        [Command(channel = Channels.Unreliable, requiresAuthority = true)]
        private void CmdResetState()
        {
            RpcResetState();
        }

#if SELF_TEST_VOICE
        [ClientRpc(channel = Channels.Unreliable, includeOwner = true)]
#else
        [ClientRpc(channel = Channels.Unreliable, includeOwner = false)]
#endif
        private void RpcReceiveVoice(byte[] opusData)
        {
            if (_opusDecoderThreaded == null)
            {
                Debug.LogError("Failed to decode voice because decoder isn't available");
                return;
            }

            _opusDecoderThreaded.AddToDecodeQueue(opusData);
        }

#if SELF_TEST_VOICE
        [ClientRpc(channel = Channels.Unreliable, includeOwner = true)]
#else
        [ClientRpc(channel = Channels.Unreliable, includeOwner = false)]
#endif
        private void RpcResetState()
        {
            _opusDecoderThreaded?.ResetState();
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

        public void OnGUI()
        {
            GUI.HorizontalSlider(new Rect(100, 600, 200, 24), micLevel, 0f, 1f);
            GUI.TextArea(new Rect(100, 624, 100, 24), micTalking ? "talking!": "");
        }
    }
}