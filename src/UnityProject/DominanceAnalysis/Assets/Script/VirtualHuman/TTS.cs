﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;

public class TTS : MonoBehaviour
{
    private GCTextToSpeech _gcTextToSpeech;
    public Queue<AudioClip> audioQueue;
    public AudioSource audioSource;

    private readonly ManualResetEvent stoppeing_event_ = new ManualResetEvent(false);

    public string ReceiveString;
    public string msg_server;
    // float num = 0;

    public int volume;
    public int pitch;

    public string avatar_name;

    public event Action OnPlaybackCompleted;

    void Start()
    {
        _gcTextToSpeech = GCTextToSpeech.Instance;
        _gcTextToSpeech.SynthesizeSuccessEvent += _gcTextToSpeech_SynthesizeSuccessEvent;
        _gcTextToSpeech.SynthesizeFailedEvent += _gcTextToSpeech_SynthesizeFailedEvent;
        audioQueue = new Queue<AudioClip>();

        // TextToSpeech("날씨가 갑자기 추워졌네요.");
    }

    // recognize success event, recognize failed event disable function
    public void EventDisable()
    {
        _gcTextToSpeech.SynthesizeSuccessEvent -= _gcTextToSpeech_SynthesizeSuccessEvent;
        _gcTextToSpeech.SynthesizeFailedEvent -= _gcTextToSpeech_SynthesizeFailedEvent;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    // STT stt = GameObject.Find("TTSSTT").GetComponent<STT>();
    //    STT stt = GetComponent<STT>();

    //    ReceiveString = "";
    //    msg_server = "";

    //    if (ReceiveString != stt.ReceiveString)
    //    {
    //        if (stt.ReceiveString.Length >= 1)
    //        {
    //            num += 1;
    //            Debug.Log(stt.ReceiveString);
    //            StartCoroutine(TextToSpeechCoroutine(stt.ReceiveString));
    //            if (num >= 1)
    //            {
    //                stt.ReceiveString = "";
    //            }
    //        }
    //    }
    //}

    //private IEnumerator TextToSpeechCoroutine(string text)
    //{
    //    yield return TextToSpeechAsync(text);
    //}

    public void SetVolumePitch((int volume, int pitch) vpTuple)
    {
        this.volume = vpTuple.volume;
        this.pitch = vpTuple.pitch;
    }

    public async void PlayDialog(string dialog)
    {
        await TextToSpeechAsync(dialog);

        StartCoroutine(WaitForPlaybackToEnd());
    }

    private IEnumerator WaitForPlaybackToEnd()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        OnPlaybackCompleted?.Invoke();
    }

    public async Task TextToSpeechAsync(string sentence)
    {
        AudioSource audio = GetComponent<AudioSource>();

        string client_id = "";
        string client_secret = ""; // naver clova token

        string url = "https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts"; // naver clova api url
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers.Add("X-NCP-APIGW-API-KEY-ID", client_id);
        request.Headers.Add("X-NCP-APIGW-API-KEY", client_secret);
        request.Method = "POST";

        // volume과 pitch 출력
        Debug.Log($"volume: {volume}, pitch: {pitch}");

        byte[] byteDataParams = Encoding.UTF8.GetBytes($"speaker={avatar_name}&volume={volume}&speed=0&pitch={pitch}&alpha=-1&format=wav&text={sentence}");

        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteDataParams.Length;

        using (Stream st = await request.GetRequestStreamAsync())
        {
            await st.WriteAsync(byteDataParams, 0, byteDataParams.Length);
        }

        HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

        Stream input = response.GetResponseStream();

        var memoryStream = new MemoryStream();
        await input.CopyToAsync(memoryStream);
        byte[] byteArray = memoryStream.ToArray();
        float[] f = ConvertByteToFloat(byteArray);

        using (Stream s = new MemoryStream(byteArray))
        {
            AudioClip audioClip = AudioClip.Create("ttsaudio", f.Length, 1, 24000, false);
            audioClip.SetData(f, 0);
            audio.clip = audioClip;
            audio.Play();
        }
    }
    private float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 2];
        for (int i = 0; i < floatArr.Length; i++)
        {
            floatArr[i] = BitConverter.ToInt16(array, i * 2) / 32768.0f;
        }
        return floatArr;
    }

    [DllImport("user32.dll")]
    public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    [DllImport("user32")]
    public static extern int SetCursorPos(int x, int y);

    #region failed handlers

    private void _gcTextToSpeech_SynthesizeFailedEvent(string error)
    {
        //Debug.Log(error);
        //GCSpeechRecognition.instance.TextToSpeechWork();
    }

    #endregion failed handlers

    #region sucess handlers

    private void _gcTextToSpeech_SynthesizeSuccessEvent(PostSynthesizeResponse response)
    {
        try
        {
            AudioClip clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
            audioSource.clip = clip;

            audioQueue.Enqueue(clip);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    #endregion sucess handlers
}