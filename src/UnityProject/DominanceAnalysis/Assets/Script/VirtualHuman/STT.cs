using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
public class STT : MonoBehaviour
{
    private GCSpeechRecognition _speechRecognition;
    public int ReceivedataLength;
    public byte[] Receivebyte = new byte[2000];
    private string transcript;
    public string ReceiveString;

    public event Action<string> OnTranscriptReceived;

    // 이 STT를 포함하는 객체 내에 있는 VirtualHumanController 객체
    public VirtualHumanController virtualHumanController;

    void Start()
    {
        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
        _speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;

        _speechRecognition.EndTalkigEvent += TalkEndedEventHandler;

        virtualHumanController = GetComponent<VirtualHumanController>();
    }

    // recognize success event, recognize failed event disable function
    public void EventDisable()
    {
        _speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
        _speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;
    }

    public void StartRecord()
    {
        _speechRecognition.StartRecord(false);
    }

    public void StopRecord()
    {
        _speechRecognition.StopRecord();
    }
    
    private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
    {
        if (recognitionResponse == null || recognitionResponse.results.Length == 0)
        {
            Debug.Log("null");
        }

        else
        {
            transcript = recognitionResponse.results[0].alternatives[0].transcript;
            OnTranscriptReceived?.Invoke(transcript);
        }
    }


    private void RecognizeFailedEventHandler(string error)
    {
        Debug.Log("recognized Fail" + error);
    }

    void TalkEndedEventHandler(AudioClip clip, float[] rawData)
    {
        //로그
        Debug.Log("TalkEndedEventHandler");

        // virtualHumanController가 유효한가
        if (virtualHumanController == null)
        {
            Debug.Log("VirtualHumanController is null");
            return;
        }
        virtualHumanController.PlayBackChannel();
    }
}