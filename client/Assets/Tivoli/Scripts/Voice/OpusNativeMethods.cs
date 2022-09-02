using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tivoli.Scripts.Voice
{
    public class OpusNativeMethods
    {
        public enum OpusErrors
        {
            Ok = 0,
            BadArgument = -1,
            BufferTooSmall = -2,
            InternalError = -3,
            InvalidPacket = -4,
            NotImplemented = -5,
            InvalidState = -6,
            AllocFail = -7
        }

        public enum OpusApplication
        {
            Voip = 2048,
            Audio = 2049,
            RestrictedLowDelay = 2051
        }

        public enum OpusCtl
        {
            SetApplicationRequest = 4000,
            GetApplicationRequest = 4001,
            SetBitrateRequest = 4002,
            GetBitrateRequest = 4003,
            SetMaxBandwidthRequest = 4004,
            GetMaxBandwidthRequest = 4005,
            SetVbrRequest = 4006,
            GetVbrRequest = 4007,
            SetBandwidthRequest = 4008,
            GetBandwidthRequest = 4009,
            SetComplexityRequest = 4010,
            GetComplexityRequest = 4011,
            SetInbandFecRequest = 4012,
            GetInbandFecRequest = 4013,
            SetPacketLossPercRequest = 4014,
            GetPacketLossPercRequest = 4015,
            SetDtxRequest = 4016,
            GetDtxRequest = 4017,
            SetVbrConstraintRequest = 4020,
            GetVbrConstraintRequest = 4021,
            SetForceChannelsRequest = 4022,
            GetForceChannelsRequest = 4023,
            SetSignalRequest = 4024,
            GetSignalRequest = 4025,
            GetLookaheadRequest = 4027,
            ResetState = 4028,
            GetSampleRateRequest = 4029,
            GetFinalRangeRequest = 4031,
            GetPitchRequest = 4033,
            SetGainRequest = 4034,
            GetGainRequest = 4045, // should have been 4035
            SetLsbDepthRequest = 4036,
            GetLsbDepthRequest = 4037,
            GetLastPacketDurationRequest = 4039,
            SetExpertFrameDurationRequest = 4040,
            GetExpertFrameDurationRequest = 4041,
            SetPredictionDisabledRequest = 4042,
            GetPredictionDisabledRequest = 4043,

            // don't use 4045, it's already taken by OPUS_GET_GAIN_REQUEST
            SetPhaseInversionDisabledRequest = 4046,
            GetPhaseInversionDisabledRequest = 4047
        }

        private const string PluginName = "opus";

        // encoding

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encoder_get_size(int numChannels);

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern OpusErrors opus_encoder_init(IntPtr encoder, int sampleRate, int channelCount,
            int application);

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int opus_encode_float(IntPtr st, float[] pcm, int frame_size, byte[] data,
            int max_data_bytes);

        // encoder controlling

        // get values
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_encoder_ctl(IntPtr encoder, OpusCtl request, out int value);
        // set values
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_encoder_ctl(IntPtr encoder, OpusCtl request, int value);
        // reset values
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int opus_encoder_ctl(IntPtr encoder, OpusCtl request);

        // decoding

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decoder_get_size(int numChannels);

        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern OpusErrors opus_decoder_init(IntPtr decoder, int sampleRate, int channelCount);
        
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int opus_decode_float(IntPtr decoder, byte[] data, int len, float[] pcm, int frameSize, int useFEC);
        // useFEC means to use in-band forward error correction!

        
        // decoder controlling
        // get values
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_decoder_ctl(IntPtr decoder, OpusCtl request, out int value);
        // set values
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_decoder_ctl(IntPtr decoder, OpusCtl request, int value);
        // reset values
        [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int opus_decoder_ctl(IntPtr decoder, OpusCtl request);


        internal static IntPtr opus_encoder_create(int sampleRate, int channelCount, OpusApplication application,
            out OpusErrors error)
        {
            var size = opus_encoder_get_size(channelCount);
            var ptr = Marshal.AllocHGlobal(size);

            error = opus_encoder_init(ptr, sampleRate, channelCount, (int) application);

            if (error == OpusErrors.Ok) return ptr;
            if (ptr == IntPtr.Zero) return ptr;

            destroy_opus(ptr);
            ptr = IntPtr.Zero;

            return ptr;
        }

        internal static int opus_encode(IntPtr encoder, float[] pcmData, int frameSize, byte[] encodedData)
        {
            if (encoder == IntPtr.Zero)
            {
                Debug.LogError("Encoder not initialized");
                return 0;
            }

            var byteLength = opus_encode_float(encoder, pcmData, frameSize, encodedData, encodedData.Length);

            if (byteLength > 0) return byteLength;

            Debug.LogError("Encoding error: " + (OpusErrors) byteLength);
            Debug.Log("Input PCM length: " + pcmData.Length);

            return byteLength;
        }
        
        internal static int opus_reset_encoder(IntPtr encoder)
        {
            if (encoder == IntPtr.Zero)
            {
                Debug.LogError("Encoder not initialized");
                return 0;
            }

            var resp = opus_encoder_ctl(encoder, OpusCtl.ResetState);
            if (resp != 0)
            {
                Debug.LogError("Resetting encoder had response: " + resp);
            }

            return resp;
        }
        
        internal static IntPtr opus_decoder_create(int sampleRate, int channelCount, out OpusErrors error)
        {
            var decoderSize = opus_decoder_get_size(channelCount);
            var ptr = Marshal.AllocHGlobal(decoderSize);
            error = opus_decoder_init(ptr, sampleRate, channelCount);
            return ptr;
        }
        
        internal static int opus_decode(IntPtr decoder, byte[] encodedData, float[] outputPcm, int channelSampleRate, int channelCount)
        {
            if (decoder == IntPtr.Zero)
            {
                Debug.LogError("Encoder empty??");
                return 0;
            }
            
            const int useForwardErrorCorrection = 0;

            var length = opus_decode_float(decoder,
                encodedData,
                encodedData.Length,
                outputPcm,
                // encodedData == null ? channelSampleRate / 100 * channelCount : outputPcm.Length / channelCount,
                outputPcm.Length / channelCount,
                useForwardErrorCorrection);

            if (length <= 0)
            {
                Debug.LogError("Decoding error: " + (OpusErrors)length);
            }

            return length * channelCount;
        }
        
        internal static int opus_reset_decoder(IntPtr decoder)
        {
            if (decoder == IntPtr.Zero)
            {
                Debug.LogError("Decoder not initialized");
                return 0;
            }

            var resp = opus_decoder_ctl(decoder, OpusCtl.ResetState);
            if (resp != 0)
            {
                Debug.LogError("Resetting decoder had response: " + resp);
            }
            
            return resp;
        }

        internal static void destroy_opus(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}