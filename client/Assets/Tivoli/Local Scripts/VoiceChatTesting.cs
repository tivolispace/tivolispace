using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceChatTesting : MonoBehaviour
{
    private AudioClip mic;

    private AudioClip testClip;
    private AudioSource testSource;

    private const int SampleRate = 48000;

    private int lastPos, pos;

    // private readonly VoicePlaybackBuffer _decoder = new();
    
    public Action<float[], float> OnAudioSample;

    public void Awake()
    {
        // testClip = AudioClip.Create(
        //     "Test Clip",
        //     SampleRate,
        //     1,
        //     SampleRate,
        //     true,
        //     data =>
        //     {
        //         _decoder.Read(data, 0, data.Length);
        //     }
        // );

        AudioSettings.outputSampleRate = 48000;
        AudioSettings.speakerMode = AudioSpeakerMode.Mono;
            
        testSource = GetComponent<AudioSource>();
        // testSource.clip = testClip;
        // testSource.loop = true;
        testSource.Play();

        mic = Microphone.Start(null, true, 5, SampleRate);
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        // var monoData = new float[stereoData.Length / 2]; 
        //
        // var numRead = _decoder.Read(monoData, 0, monoData.Length);
        // var percentUnderRun = 1f - (float) numRead / monoData.Length;
        // // Destroy();
        //
        // for (var i = 0; i < monoData.Length; i++)
        // {
        //     stereoData[i * 2] = monoData[i];
        //     stereoData[i * 2 + 1] = monoData[i];
        // }
        //
        // OnAudioSample?.Invoke(stereoData, percentUnderRun);

        // var numRead = _decoder.Read(data, 0, data.Length);
        // var percentUnderRun = 1f - (float) numRead / data.Length;
        //
        // OnAudioSample?.Invoke(data, percentUnderRun);
    }

    private void SendToServer(float[] samples)
    {
        if (samples == null || samples.Length == 0) return;
    
        // _decoder.AddDecodedAudio(samples);

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
            if (lastPos > pos)
            {
                // mic loop reset
                lastPos = 0;
            }
            
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