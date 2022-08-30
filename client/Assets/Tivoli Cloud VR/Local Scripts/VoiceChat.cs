using System;
using System.Collections.Generic;
using UnityEngine;

public class VoiceChat : MonoBehaviour
{
    private AudioClip mic;

    private AudioClip testClip;
    private AudioSource testSource;

    private const int SampleRate = 44100;
    // private const int BufferLength = (int) (SampleRate * 0.1f); // 100 ms
    // // >--------o-->  <- play head
    // //    `-----`  <- buffer length
    // // ^     ^ try to be in the center
    // // but move when here

    private int lastPos, pos;

    private List<float> streamDataList = new();
    private int streamPosition;
    
    public void Awake()
    {
        // var wormByteData = File.ReadAllBytes(Path.Combine(Application.dataPath, "Tivoli Cloud VR/f32le-44100-1.raw"));
        // var wormFloatData = new float[wormByteData.Length / 4];
        // Buffer.BlockCopy(wormByteData, 0, wormFloatData, 0, wormByteData.Length);
        
        testClip = AudioClip.Create(
            "Test Clip",
            SampleRate,
            1,
            SampleRate,
            true,
            data =>
            {
                var streamData = streamDataList.ToArray();
                
                Debug.Log("played: " + streamPosition/streamData.Length * 100 + "% (" + streamData.Length + ")");
                
                if (streamPosition == streamData.Length)
                {
                    streamPosition = 0;
                }

                // // make sure to always be one second behind
                // 
                // if (lastRead > streamData.Length - bufferInSamples)
                // {
                //     lastRead -= bufferInSamples;
                // }
                //
                // if (lastRead < 0)
                // {
                //     lastRead = 0;
                //     return;
                // }

                var lengthCanCopy = streamPosition + data.Length > streamData.Length - 1
                    ? streamData.Length - streamPosition
                    : data.Length;

                Array.Copy(
                    streamData, streamPosition,
                    data, 0,
                    lengthCanCopy
                );

                if (lengthCanCopy < data.Length)
                {
                    // fill the rest with zeros
                    Array.Clear(data, lengthCanCopy, data.Length - lengthCanCopy);
                }
                
                streamPosition += lengthCanCopy;
            }
        );

        testSource = GetComponent<AudioSource>();
        testSource.clip = testClip;
        testSource.loop = true;
        testSource.Play();

        mic = Microphone.Start(null, true, 1, SampleRate);
    }

    private void SendToServer(float[] samples)
    {
        if (samples == null || samples.Length == 0) return;

        // convert samples from f32le (float) to u16le (ushort)
        // var samplesUint = new ushort[samples.Length];
        // for (var i = 0; i < samples.Length; i++)
        // {
        //     samplesUint[i] = BitConverter.ToUInt16(BitConverter.GetBytes(samples[i] * float.MaxValue));
        // }

        // streamDataList.AddRange(samples);
        
        // decodedAudioBuffer.AddDecodedAudio(samples, samples.Length, );
        
    }

    private void Update()
    {
        if ((pos = Microphone.GetPosition(null)) > 0)
        {
            if (lastPos > pos) lastPos = 0;
            if (pos - lastPos > 0)
            {
                var length = pos - lastPos;
                var samples = new float[length];
                mic.GetData(samples, lastPos);
                SendToServer(samples);
                lastPos = pos;
            }
        }
    }

    private void OnDestroy()
    {
        Microphone.End(null);
    }
}