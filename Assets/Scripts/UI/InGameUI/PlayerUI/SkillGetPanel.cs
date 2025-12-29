using System.Collections;
using UnityEngine;

public class SkillGetPanel : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private CanvasGroup _dimBackground;   // 검정 배경
    [SerializeField] private RectTransform _infoPanel;     // 알림창
    [SerializeField] private CanvasGroup _infoCanvasGroup; // 알림창의 CanvasGroup

    [Header("연출 설정")]
    [SerializeField] float _displayDuration = 3f;  // 지속시간
    [SerializeField] float _fadeDuration = 0.5f;   // 페이드 지속시간
    [SerializeField] Vector2 _startOffset = new Vector2(0, -300f); // 시작 위치 오프셋

    private Vector2 _targetAnchoredPos;

    private void Awake()
    {
        _targetAnchoredPos = _infoPanel.anchoredPosition;
    }

    private void OnEnable()
    {
        // 켜질 때마다 연출 재생
        StopAllCoroutines();
        StartCoroutine(ShowSequence());
    }

    IEnumerator ShowSequence()
    {
        // 초기화
        if (_dimBackground != null) _dimBackground.alpha = 0f;
        _infoCanvasGroup.alpha = 0f;
        _infoPanel.anchoredPosition = _targetAnchoredPos + _startOffset;

        //슬라이드, 페이드인
        float t = 0;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            float progress = t / _fadeDuration;

            // 배경 페이드
            if (_dimBackground != null) _dimBackground.alpha = progress * 0.8f;

            // 알림창 페이드 + 이동
            _infoCanvasGroup.alpha = progress;
            _infoPanel.anchoredPosition = Vector2.Lerp(_infoPanel.anchoredPosition, _targetAnchoredPos, Mathf.SmoothStep(0, 1, progress));

            yield return null;
        }

        yield return new WaitForSeconds(_displayDuration);

        // 페이드 아웃
        t = 0;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            float outProgress = 1 - (t / _fadeDuration);

            if (_dimBackground != null) _dimBackground.alpha = outProgress * 0.8f;
            _infoCanvasGroup.alpha = outProgress;

            yield return null;
        }

        // 연출 끝난 후 오브젝트 끄기
        gameObject.SetActive(false);
    }
}
