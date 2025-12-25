using System.Collections;
using UnityEngine;

public class StageBackgroundChanger : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("배경 이미지를 담당하는 스프라이트 렌더러")]
    [SerializeField] private SpriteRenderer _backgroundRenderer;

    [Header("Settings")]
    [Tooltip("변경할 다음 스테이지 배경 이미지")]
    [SerializeField] private Sprite _nextBackgroundSprite;
    [Tooltip("페이드 아웃/인에 걸리는 시간 (초)")]
    [SerializeField] private float _fadeDuration = 1.5f;

    private void Awake()
    {
        if (_backgroundRenderer == null)
            _backgroundRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 외부 이벤트(보스 사망)에서 호출할 함수
    /// </summary>
    public void ChangeBackground()
    {
        if (_nextBackgroundSprite == null)
        {
            Debug.LogError("교체할 배경 스프라이트가 설정되지 않았습니다.");
            return;
        }

        StartCoroutine(ProcessBackgroundChange());
    }

    private IEnumerator ProcessBackgroundChange()
    {
        Color originalColor = _backgroundRenderer.color;
        float timer = 0f;

        // 1. Fade Out (알파값 1 -> 0)
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / _fadeDuration);
            _backgroundRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 2. 이미지 교체
        _backgroundRenderer.sprite = _nextBackgroundSprite;

        // 3. Fade In (알파값 0 -> 1)
        timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / _fadeDuration);
            _backgroundRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 확실하게 알파값 1로 고정
        _backgroundRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }
}