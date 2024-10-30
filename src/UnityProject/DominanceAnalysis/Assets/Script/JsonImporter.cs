using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GestureDataLoader : MonoBehaviour
{
    public VirtualHumanData virtualHumanData; // ScriptableObject 참조
    public TextAsset[] jsonFiles; // JSON 파일들을 할당할 배열

    void Start()
    {
        LoadGestureData();
    }

    void LoadGestureData()
    {
        if (virtualHumanData == null)
        {
            Debug.LogError("VirtualHumanData ScriptableObject가 할당되지 않았습니다.");
            return;
        }

        if (jsonFiles == null || jsonFiles.Length == 0)
        {
            Debug.LogError("JSON 파일이 할당되지 않았습니다.");
            return;
        }

        List<VirtualHumanData.MonologueData> monologueDataList = new List<VirtualHumanData.MonologueData>();

        foreach (TextAsset jsonFile in jsonFiles)
        {
            if (jsonFile == null)
            {
                continue;
            }

            // JSON 데이터 파싱
            GestureList gestureList = JsonUtility.FromJson<GestureList>(jsonFile.text);

            if (gestureList != null && gestureList.gesture != null)
            {
                // 새로운 MonologueData 생성
                VirtualHumanData.MonologueData monologueData = new VirtualHumanData.MonologueData
                {
                    monologueAudio = null, // 필요한 경우 AudioClip 할당
                    monologueGestures = gestureList.gesture
                };

                monologueDataList.Add(monologueData);
            }
            else
            {
                Debug.LogWarning("JSON 파일에서 제스처 데이터를 파싱하지 못했습니다: " + jsonFile.name);
            }
        }

        // ScriptableObject에 로드된 monologueData 할당
        virtualHumanData.monologueDatas = monologueDataList.ToArray();

        // 데이터 지속성 처리 (선택 사항)
        // 런타임 중에 ScriptableObject의 변경 사항은 에셋 파일에 저장되지 않으므로,
        // 데이터가 필요할 때마다 JSON 파일로부터 로드하는 방식을 유지합니다.
    }
}

[System.Serializable]
public class GestureList
{
    public List<GestureData> gesture;
}