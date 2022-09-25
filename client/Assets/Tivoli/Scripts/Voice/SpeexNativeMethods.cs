using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public static class SpeexNative
    {
        // manual
        // https://speex.org/docs/manual/speex-manual/node7.html

        private const string PluginName = "speexdsp";
        
        // resampling!
        // https://speex.org/docs/api/speex-api-reference/speex__resampler_8h-source.html
        
        public enum SpeexResamplerError
        {
            Success = 0,
            AllocFailed = 1,
            BadState = 2,
            InvalidArg = 3,
            PtrOverlap = 4,
        }

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr speex_resampler_init(uint channels, uint inRate, uint outRate, int quality,
            out SpeexResamplerError err);

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SpeexResamplerError speex_resampler_process_float(IntPtr resampler, uint channelIndex,
            [In] float[] inData, ref uint inLen, [Out] float[] outData, ref uint outLen);

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void speex_resampler_destroy(IntPtr resampler);

        // speex_resampler_process_interleaved_float would help with multi channel
        
        // preprocessing!
        // https://speex.org/docs/api/speex-api-reference/speex__preprocess_8h-source.html
        
        public enum SpeexPreprocessRequest
        {
            SetDenoise = 0,
            GetDenoise = 1,
            SetAgc = 2,
            GetAgc = 3,
            SetVad = 4,
            GetVad = 5,
            SetAgcLevel = 6,
            GetAgcLevel = 7,
            SetDereverb = 8,
            GetDereverb = 9,
            SetDereverbLevel = 10,
            GetDereverbLevel = 11,
            SetDereverbDecay = 12,
            GetDereverbDecay = 13,
            SetProbStart = 14,
            GetProbStart = 15,
            SetProbContinue = 16,
            GetProbContinue = 17,
            SetNoiseSuppress = 18,
            GetNoiseSuppress = 19,
            SetEchoSuppress = 20,
            GetEchoSuppress = 21,
            SetEchoSuppressActive = 22,
            GetEchoSuppressActive = 23,
            SetEchoState = 24,
            GetEchoState = 25,
            SetAgcIncrement = 26,
            GetAgcIncrement = 27,
            SetAgcDecrement = 28,
            GetAgcDecrement = 29,
            SetAgcMaxGain = 30,
            GetAgcMaxGain = 31,
            GetAgcLoudness = 33,
            GetAgcGain = 35,
            GetPsdSize = 37,
            GetPsd = 39,
            GetNoisePsdSize = 41,
            GetNoisePsd = 43,
            GetProb = 45,
            SetAgcTarget = 46,
            GetAgcTarget = 47,
        }
        
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr speex_preprocess_state_init(int frameSize, int samplingRate);
        
        // heck no float function!!?
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int speex_preprocess_run(IntPtr preprocessState, [In, Out] short[] inData);
        
        // [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        // public static extern void speex_preprocess_estimate_update(IntPtr preprocessState, [In] short[] inData);
        
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int speex_preprocess_ctl(IntPtr preprocessState, SpeexPreprocessRequest request, ref int ptr);
        
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int speex_preprocess_ctl(IntPtr preprocessState, SpeexPreprocessRequest request, ref float ptr);
        
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void speex_preprocess_state_destroy(IntPtr preprocessState);
    }
}