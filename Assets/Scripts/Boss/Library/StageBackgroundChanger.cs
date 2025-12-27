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
    [Tooltip("크로스페이드에 걸리는 시간 (초)")]
    [SerializeField] private float _fadeDuration = 1.5f;

    private Sprite _originalSprite;      // 원래 배경 이미지 저장용
    private GameObject _tempObject;      // 크로스페이드용 임시 객체

    private void Awake()
    {
        if (_backgroundRenderer == null)
            _backgroundRenderer = GetComponent<SpriteRenderer>();

        if (_backgroundRenderer != null)
        {
            _originalSprite = _backgroundRenderer.sprite;
        }
    }

    /// <summary>
    /// 다음 배경으로 부드럽게 변경
    /// </summary>
    public void ChangeBackground()
    {
        if (_nextBackgroundSprite == null)
        {
            Debug.LogError("교체할 배경 스프라이트가 설정되지 않았습니다.");
            return;
        }

        // 기존 코루틴/임시객체 정리
        StopAllCoroutines();
        if (_tempObject != null) Destroy(_tempObject);

        // 다음 배경으로 크로스페이드 시작
        StartCoroutine(ProcessCrossfade(_nextBackgroundSprite));
    }

    /// <summary>
    /// 원래 배경으로 부드럽게 복구
    /// </summary>
    public void RevertBackground()
    {
        if (_originalSprite == null) return;

        // 기존 코루틴/임시객체 정리
        StopAllCoroutines();
        if (_tempObject != null) Destroy(_tempObject);

        // 원래 배경으로 크로스페이드 시작
        StartCoroutine(ProcessCrossfade(_originalSprite));
    }

    /// <summary>
    /// [플레이어 사망/리셋] 즉시 원래 배경으로 되돌림
    /// </summary>
    public void ResetBackground()
    {
        StopAllCoroutines();

        // 크로스페이드 중이었다면 임시 객체 삭제
        if (_tempObject != null)
        {
            Destroy(_tempObject);
            _tempObject = null;
        }

        // 배경 원상복구
        if (_backgroundRenderer != null && _originalSprite != null)
        {
            _backgroundRenderer.sprite = _originalSprite;

            // 투명도/색상 초기화
            Color color = _backgroundRenderer.color;
            color.a = 1f;
            _backgroundRenderer.color = color;
        }
    }

    // 목표 스프라이트를 인자로 받아서 변경/복구 모두 사용 가능하게 수정
    private IEnumerator ProcessCrossfade(Sprite targetSprite)
    {
        // 1. 임시 오브젝트 생성 (현재 보여지고 있는 배경 상태 복사)
        _tempObject = new GameObject("TempBackground");
        _tempObject.transform.position = _backgroundRenderer.transform.position;
        _tempObject.transform.rotation = _backgroundRenderer.transform.rotation;
        _tempObject.transform.localScale = _backgroundRenderer.transform.localScale;

        // 2. 임시 렌더러 설정 (목표 이미지 적용)
        SpriteRenderer tempRenderer = _tempObject.AddComponent<SpriteRenderer>();
        tempRenderer.sprite = targetSprite; // 전달받은 목표 이미지
        tempRenderer.sortingLayerID = _backgroundRenderer.sortingLayerID;
        // 기존 배경보다 한 단계 앞에서 그려지도록 설정 
        tempRenderer.sortingOrder = _backgroundRenderer.sortingOrder + 1;

        // 3. 페이드 인 (투명 -> 불투명)
        Color targetColor = _backgroundRenderer.color;
        tempRenderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0f); // 시작은 투명

        float timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / _fadeDuration);
            tempRenderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
            yield return null;
        }

        // 4. 완료 처리 (원본 렌더러를 목표 이미지로 교체하고 임시 객체 삭제)
        if (_backgroundRenderer != null)
        {
            _backgroundRenderer.sprite = targetSprite;
        }

        if (_tempObject != null)
        {
            Destroy(_tempObject);
            _tempObject = null;
        }
    }
}