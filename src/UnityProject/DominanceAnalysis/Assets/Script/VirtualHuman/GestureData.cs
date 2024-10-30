using System;

/// 제스처 데이터를 저장하는 클래스
[Serializable]
public class GestureData
{
    public string word_or_phrase;
    public string gesture;    // 제스처 타입 (예: "nod", "shake")
    public float start;       // 제스처 시작 시간
}