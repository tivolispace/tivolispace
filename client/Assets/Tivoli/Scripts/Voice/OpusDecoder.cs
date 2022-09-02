using System;
using System.Collections.Generic;

namespace Tivoli.Scripts.Voice
{
    public class OpusDecoder
    {
        private static readonly List<int> PermittedSampleRates = new()
        {
            8000, 12000, 16000, 24000, 48000
        };

        private static readonly List<int> PermittedChannels = new()
        {
            1, 2
        };

        private IntPtr _decoder;

        private int _outputSampleRate;
        private int _outputChannels;

        public OpusDecoder(int outputSampleRate, int outputChannels)
        {
            if (!PermittedSampleRates.Contains(outputSampleRate))
            {
                throw new ArgumentOutOfRangeException(nameof(outputSampleRate));
            }

            if (!PermittedChannels.Contains(outputChannels))
            {
                throw new ArgumentOutOfRangeException(nameof(outputChannels));
            }

            var decoder = OpusNativeMethods.opus_decoder_create(outputSampleRate, outputChannels, out var error);
            if (error != OpusNativeMethods.OpusErrors.Ok)
            {
                throw new Exception(
                    $"Exception occured while creating decoder: {error}");
            }

            _decoder = decoder;

            _outputSampleRate = outputSampleRate;
            _outputChannels = outputChannels;
        }
        
        public void ResetState()
        {
            OpusNativeMethods.opus_reset_decoder(_decoder); 
        }
        
        public int Decode(byte[] opusData, float[] pcmSamples)
        {
            return OpusNativeMethods.opus_decode(_decoder, opusData, pcmSamples, _outputSampleRate, _outputChannels);
        }
        
        ~OpusDecoder()
        {
            if (_decoder == IntPtr.Zero) return;
            OpusNativeMethods.destroy_opus(_decoder);
            _decoder = IntPtr.Zero;
        }
    }
}