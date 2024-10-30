using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFade : MonoBehaviour
{
    public Image blackScreen;  // 검은 화면 이미지
    public float fadeDuration = 2.0f;  // 페이드 시간

    private void Start()
    {
    }

    public void StartFadeIn()
    {
        StartCoroutine(Fade(false));
    }

    public void StartFadeOut()
    {
        StartCoroutine(Fade(true));
    }

    private IEnumerator Fade(bool black)
    {
        float timer = 0f;
        Color color = blackScreen.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(black ? 0 : 1, black ? 1 : 0, timer / fadeDuration);  // 알파(투명도)값 시간으로 계산
            blackScreen.color = new Color(color.r, color.g, color.b, alpha);  // 알파값 변경
            yield return null;
        }
        blackScreen.color = new Color(color.r, color.g, color.b, black ? 1 : 0);  // 최종 알파값 설정
    }
}