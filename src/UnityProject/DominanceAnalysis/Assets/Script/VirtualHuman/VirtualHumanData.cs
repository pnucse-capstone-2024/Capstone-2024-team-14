using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "VirtualHumanData", menuName = "Scriptable Object/VirtualHumanData", order = 1)]
public class VirtualHumanData : ScriptableObject
{
    public AudioClip greetingAudio;  // 입장 인사 오디오 클립
    public AudioClip farewellAudio;  // 퇴장 인사 오디오 클립

    public List<AudioClip> backChannelAudios;

    [System.Serializable]
    public struct MonologueData
    {
        public AudioClip monologueAudio;
        public List<GestureData> monologueGestures;
    }
    

    [System.Serializable]
    public struct QuestionAnswerPair
    {
        public string userQuestion;           // 사용자 질문
        public string virtualHumanAnswer;   // 가상인간의 답변
        public string virtualHumanQuestion; // 가상인간의 질문

        public List<GestureData> virtualHumanAnswerGestures;
        public List<GestureData> virtualHumanQuestionGestures;
    }

    public QuestionAnswerPair[] questionAnswerPairs = new QuestionAnswerPair[9];  // 9개의 질문-답변 쌍
    public MonologueData[] monologueDatas = new MonologueData[3];  // 3개의 독백 데이터
}
