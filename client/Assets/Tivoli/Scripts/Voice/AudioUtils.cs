using System.Collections.Generic;

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
        
        // samples to level
    }
}