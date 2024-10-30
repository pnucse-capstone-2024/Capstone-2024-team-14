using System.Collections.Generic;

public class VHResponseData
{
    public string reason { get; set; } // 응답에 대한 설명 또는 이유
    public string speech { get; set; } // 가상인간의 대사
    public List<GestureData> gesture { get; set; } // 제스처 목록
    public float speech_time { get; set; } // 대사 길이 또는 전체 대사 재생 시간
    public float backchannel { get; set; } // 백채널 빈도
}