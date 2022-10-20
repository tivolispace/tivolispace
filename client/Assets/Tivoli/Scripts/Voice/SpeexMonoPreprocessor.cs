using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class SpeexMonoPreprocessor
    {
        private readonly IntPtr preprocessor;

        public SpeexMonoPreprocessor(int frameSize, int samplingRate)
        {
            preprocessor = SpeexNative.speex_preprocess_state_init(frameSize, samplingRate);
        }

        public void SetBool(SpeexNative.SpeexPreprocessRequest request, bool enabled)
        {
            var value = enabled ? 1 : 2;
            SpeexNative.speex_preprocess_ctl(preprocessor, request, ref value);
        }

        public void SetInt(SpeexNative.SpeexPreprocessRequest request, int value)
        {
            SpeexNative.speex_preprocess_ctl(preprocessor, request, ref value);
        }

        public void SetFloat(SpeexNative.SpeexPreprocessRequest request, float value)
        {
            SpeexNative.speex_preprocess_ctl(preprocessor, request, ref value);
        }

        public int GetInt(SpeexNative.SpeexPreprocessRequest request)
        {
            var value = 0;
            SpeexNative.speex_preprocess_ctl(preprocessor, request, ref value);
            return value;
        }

        public float GetFloat(SpeexNative.SpeexPreprocessRequest request)
        {
            var value = 0f;
            SpeexNative.speex_preprocess_ctl(preprocessor, request, ref value);
            return value;
        }

        public void RequestBool(SpeexNative.SpeexPreprocessRequest request, bool enabled)
        {
            var value = enabled ? 1 : 2;
            SpeexNative.speex_preprocess_ctl(preprocessor, request, ref value);
        }

        public float[] Preprocess(float[] pcmInput)
        {
            if (preprocessor == IntPtr.Zero) return null;

            var pcmData = AudioUtils.PcmFloatsToShorts(pcmInput);
            SpeexNative.speex_preprocess_run(preprocessor, pcmData);
            return AudioUtils.PcmShortsToFloats(pcmData);
        }

        public void OnDestroy()
        {
            if (preprocessor != IntPtr.Zero)
            {
                SpeexNative.speex_preprocess_state_destroy(preprocessor);
            }
        }

        ~SpeexMonoPreprocessor()
        {
            OnDestroy();
        }
    }
}