using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class Rnnoise
    {
        private const string PluginName = "rnnoise";

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int rnnoise_get_frame_size();

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr rnnoise_create(IntPtr model);

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float rnnoise_process_frame(IntPtr state, [Out] float[] output, [In] float[] input);

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rnnoise_destroy(IntPtr state);

        private readonly IntPtr _state;

        public readonly int FrameSize;

        public Rnnoise()
        {
            _state = rnnoise_create(IntPtr.Zero);
            FrameSize = rnnoise_get_frame_size();
        }

        public float Process(float[] pcmInput, float[] pcmOutput)
        {
            for (var i = 0; i < pcmInput.Length; i++)
            {
                pcmInput[i] *= short.MaxValue;
            }

            var vadProb = rnnoise_process_frame(_state, pcmOutput, pcmInput);
            for (var i = 0; i < pcmOutput.Length; i++)
            {
                pcmOutput[i] /= short.MaxValue;
            }

            return vadProb;
        }

        public void OnDestroy()
        {
            if (_state != IntPtr.Zero)
            {
                rnnoise_destroy(_state);
            }
        }

        ~Rnnoise()
        {
            OnDestroy();
        }
    }
}