using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public static class AudioUtils
    {
        public static short[] PcmFloatsToShorts(IReadOnlyList<float> floats)
        {
            var shorts = new short[floats.Count];
            for (var i = 0; i < floats.Count; i++)
            {
                shorts[i] = (short)(floats[i] * short.MaxValue);
            }
            return shorts;
        }
        
        public static float[] PcmShortsToFloats(IReadOnlyList<short> shorts)
        {
            var floats = new float[shorts.Count];
            for (var i = 0; i < shorts.Count; i++)
            {
                floats[i] = (float) shorts[i] / short.MaxValue;
            }
            return floats;
        }
        
        public static float[] StereoToMono(IReadOnlyList<float> samples)
        {
            var monoSamples = new float[samples.Count / 2];
            for (var i = 0; i < monoSamples.Length; i++)
            {
                var left = samples[i * 2];
                var right = samples[i * 2 + 1];
                monoSamples[i] = (left + right) / 2f;
            }
            
            return monoSamples;
        }
        
        public static void MonoToStereo(IReadOnlyList<float> mono, float[] stereo)
        {
            for (var i = 0; i < mono.Count; i++)
            {
                stereo[i * 2] = mono[i];
                stereo[i * 2 + 1] = mono[i];
            }
        }

        public static float Amplitude(IReadOnlyList<float> pcmData)
        {
            return Mathf.Clamp01(pcmData.Max());
        }
    }
}