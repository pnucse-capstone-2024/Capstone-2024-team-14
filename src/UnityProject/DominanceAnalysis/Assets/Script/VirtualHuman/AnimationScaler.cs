using UnityEngine;
using UnityEditor; // Editor 스크립트를 사용하기 위해 필요
using System.IO;

public class AnimationScaler : MonoBehaviour
{
    public Animator animator; // Animator를 연결할 수 있는 변수
    public AnimationClip animationClip; // 수정하려는 애니메이션 클립을 직접 지정
    public float lowMultiplier = 1.0f; // Low 배수
    public float midMultiplier = 2.0f; // Mid 배수
    public float highMultiplier = 3.0f; // High 배수
    public string[] propertyNames; // 수정하려는 프로퍼티 이름들 (ex: "Neck", "Arm")
    public bool scaleAllProperties = false; // 모든 프로퍼티를 수정할지 여부

    void Start()
    {
        // 에디터 환경에서만 동작하도록 설정
#if UNITY_EDITOR
        // 애니메이션 클립이 지정되어 있는지 확인
        if (animationClip != null)
        {
            Debug.Log("Scaling animation clip: " + animationClip.name);
            // 애니메이션 클립을 3개 복제 및 각각 배수 적용
            ScaleAndDuplicateClip(lowMultiplier, "Low");
            ScaleAndDuplicateClip(midMultiplier, "Mid");
            ScaleAndDuplicateClip(highMultiplier, "High");
        }
        else
        {
            Debug.LogError("AnimationClip is not assigned.");
        }
#endif
    }

    // 애니메이션 클립 복제 및 적용 함수
    void ScaleAndDuplicateClip(float multiplier, string suffix)
    {
#if UNITY_EDITOR
        // 애니메이션 클립 경로 찾기
        string assetPath = AssetDatabase.GetAssetPath(animationClip);
        string directory = Path.GetDirectoryName(assetPath);
        string newClipPath = Path.Combine(directory, animationClip.name + "_" + suffix + ".anim");

        // 애니메이션 클립 복제
        AnimationClip newClip = Instantiate(animationClip);
        newClip.name = animationClip.name + "_" + suffix;

        // 커브 값을 조정
        ScaleAnimationClip(newClip, multiplier);

        // 복제한 애니메이션 클립을 원래 경로에 저장
        AssetDatabase.CreateAsset(newClip, newClipPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"{suffix} scaled animation clip created at: " + newClipPath);
#endif
    }

    // 애니메이션 클립을 받아서 커브의 값을 배수로 조정하는 함수
    void ScaleAnimationClip(AnimationClip clip, float multiplier)
    {
#if UNITY_EDITOR
        // 모든 커브 바인딩을 가져옴
        foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        {
            // 모든 프로퍼티를 수정하는 경우
            if (scaleAllProperties || ShouldScaleProperty(binding.propertyName))
            {
                // 커브 데이터를 가져와 수정
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                // 커브의 키 프레임 값을 배수로 수정
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    Keyframe key = curve.keys[i];
                    key.value *= multiplier; // 배수로 키 프레임 값을 조정
                    curve.MoveKey(i, key); // 수정된 키 프레임을 다시 설정
                }

                // 수정된 커브를 애니메이션 클립에 적용
                AnimationUtility.SetEditorCurve(clip, binding, curve);
                Debug.Log("Scaled curve: " + binding.propertyName);
            }
        }
#endif
    }

    // 특정 프로퍼티를 수정할지 여부를 판단하는 함수
    bool ShouldScaleProperty(string propertyName)
    {
        foreach (string name in propertyNames)
        {
            if (propertyName.Contains(name))
            {
                return true;
            }
        }
        return false;
    }
}
