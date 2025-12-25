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

        StartCoroutine(ProcessCrossfade());
    }

    private IEnumerator ProcessCrossfade()
    {
        // 1. 임시 오브젝트 생성 (현재 배경 위치/크기 복사)
        GameObject tempObj = new GameObject("TempBackground");
        tempObj.transform.position = _backgroundRenderer.transform.position;
        tempObj.transform.rotation = _backgroundRenderer.transform.rotation;
        tempObj.transform.localScale = _backgroundRenderer.transform.localScale;

        // 2. 임시 렌더러 설정 (다음 배경 이미지 적용)
        SpriteRenderer tempRenderer = tempObj.AddComponent<SpriteRenderer>();
        tempRenderer.sprite = _nextBackgroundSprite;
        tempRenderer.sortingLayerID = _backgroundRenderer.sortingLayerID;
        // 기존 배경보다 한 단계 앞에서 그려지도록 설정하여 겹쳐 보이게 함
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

        // 4. 완료 처리 (원본 렌더러의 이미지를 교체하고 임시 객체 삭제)
        _backgroundRenderer.sprite = _nextBackgroundSprite;
        Destroy(tempObj);
    }
}