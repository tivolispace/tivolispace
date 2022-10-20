using System;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class SpeexMonoResampler
    {
        // The quality parameter is useful for controlling the quality/complexity/latency tradeoff.
        // Using a higher quality setting means less noise/aliasing, a higher complexity and a higher latency.
        // Usually, a quality of 3 is acceptable for most desktop uses and quality 10 is mostly recommended for pro audio work.
        // Quality 0 usually has a decent sound (certainly better than using linear interpolation resampling), but artifacts may be heard.
        //  #define SPEEX_RESAMPLER_QUALITY_MAX 10
        // #define SPEEX_RESAMPLER_QUALITY_MIN 0
        // #define SPEEX_RESAMPLER_QUALITY_DEFAULT 4
        // #define SPEEX_RESAMPLER_QUALITY_VOIP 3
        // #define SPEEX_RESAMPLER_QUALITY_DESKTOP 5
        private const int Quality = 3;

        private readonly IntPtr resampler;

        public SpeexMonoResampler(int inputSampleRate, int outputSampleRate)
        {
            resampler = SpeexNative.speex_resampler_init(1, (uint) inputSampleRate, (uint) outputSampleRate, Quality,
                out var error);
            if (error != SpeexNative.SpeexResamplerError.Success)
            {
                Debug.LogError("Failed to create resampler: " + error);
            }
        }

        public int Resample(float[] pcmInput, float[] pcmOutput)
        {
            if (resampler == IntPtr.Zero) return 0;

            var inLen = (uint) pcmInput.Length;
            var outLen = (uint) pcmOutput.Length;

            var error = SpeexNative.speex_resampler_process_float(resampler, 0, pcmInput, ref inLen, pcmOutput,
                ref outLen);
            if (error != SpeexNative.SpeexResamplerError.Success)
            {
                Debug.LogWarning("Dropping because failed to resample: " + error);
            }

            return (int) outLen;
        }

        public void OnDestroy()
        {
            if (resampler != IntPtr.Zero)
            {
                SpeexNative.speex_resampler_destroy(resampler);
            }
        }

        ~SpeexMonoResampler()
        {
            OnDestroy();
        }
    }
}