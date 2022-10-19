using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class OpusEncoder
    {
        private static readonly List<int> PermittedSampleRates = new()
        {
            8000, 12000, 16000, 24000, 48000
        };

        private static readonly List<int> PermittedChannels = new()
        {
            1, 2
        };

        private IntPtr _encoder;
        private readonly int _sampleSize;

        private const int MaxPacketSize = 1000; // stay within MTU of 1500
        private readonly byte[] _encodedPacket = new byte[MaxPacketSize];

        // https://wiki.xiph.org/Opus_Recommended_Settings
        // 64 Kb/s is the default
        // 1 channel 24 Kb/s gives full band, recommended for voice
        private const int Bitrate = 24000;

        public OpusEncoder(int inputSampleRate, int inputChannels)
        {
            if (!PermittedSampleRates.Contains(inputSampleRate))
            {
                throw new ArgumentOutOfRangeException(nameof(inputSampleRate));
            }

            if (!PermittedChannels.Contains(inputChannels))
            {
                throw new ArgumentOutOfRangeException(nameof(inputSampleRate));
            }

            var encoder = OpusNativeMethods.opus_encoder_create(inputSampleRate, inputChannels,
                OpusNativeMethods.OpusApplication.Voip, out var error);

            if (error != OpusNativeMethods.OpusErrors.Ok)
            {
                throw new Exception("Exception occured while creating encoder");
            }

            _encoder = encoder;

            const int bitDepth = 16;
            _sampleSize = SampleSize(bitDepth, inputChannels);

            // 64 KB/s is the default but we can change it here
            OpusNativeMethods.opus_encoder_ctl(_encoder, OpusNativeMethods.OpusCtl.SetBitrateRequest, Bitrate);
        }

        private static int SampleSize(int bitDepth, int channelCount)
        {
            return bitDepth / 8 * channelCount;
        }

        public byte[] Encode(float[] pcmSamples)
        {
            if (_encoder == IntPtr.Zero) return Array.Empty<byte>();

            var size = OpusNativeMethods.opus_encode(_encoder, pcmSamples, pcmSamples.Length, _encodedPacket);

            if (size > 1) return new ArraySegment<byte>(_encodedPacket, 0, size).ToArray();

            Debug.LogError("Negative size in encoded opus data");
            return Array.Empty<byte>();
        }

        public int FrameSizeInBytes(int frameSizeInSamples)
        {
            return frameSizeInSamples * _sampleSize;
        }

        public void ResetState()
        {
            if (_encoder == IntPtr.Zero) return;
            OpusNativeMethods.opus_reset_encoder(_encoder);
        }

        ~OpusEncoder()
        {
            if (_encoder == IntPtr.Zero) return;
            OpusNativeMethods.destroy_opus(_encoder);
            _encoder = IntPtr.Zero;
        }
    }
}