using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Opus : MonoBehaviour
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
    
    const string PluginName = "opus";
    
    [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern OpusErrors opus_encoder_init(IntPtr encoder, int sampleRate, int channelCount, int application);
    
    [DllImport(PluginName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int opus_encode_float(IntPtr st, float[] pcm, int frame_size, byte[] data, int max_data_bytes);
    
    void Awake()
    {
        // Debug.Log(opus_encode_float);
    }
}
