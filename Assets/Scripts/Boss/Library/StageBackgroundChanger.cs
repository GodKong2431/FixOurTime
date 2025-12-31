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

    private Sprite _originalSprite;
    private GameObject _tempObject;

    private void Awake()
    {
        if (_backgroundRenderer == null)
            _backgroundRenderer = GetComponent<SpriteRenderer>();

        if (_backgroundRenderer != null)
        {
            _originalSprite = _backgroundRenderer.sprite;
        }
    }

    public void ChangeBackground()
    {
        if (_nextBackgroundSprite == null)
        {
            Debug.LogError("교체할 배경 스프라이트가 설정되지 않았습니다.");
            return;
        }

        StopAllCoroutines();
        if (_tempObject != null) Destroy(_tempObject);

        StartCoroutine(ProcessCrossfade(_nextBackgroundSprite));
    }

    public void RevertBackground()
    {
        if (_originalSprite == null) return;

        StopAllCoroutines();
        if (_tempObject != null) Destroy(_tempObject);

        StartCoroutine(ProcessCrossfade(_originalSprite));
    }

    public void ResetBackground()
    {
        StopAllCoroutines();

        if (_tempObject != null)
        {
            Destroy(_tempObject);
            _tempObject = null;
        }

        if (_backgroundRenderer != null && _originalSprite != null)
        {
            _backgroundRenderer.sprite = _originalSprite;

            // 리셋 시에도 사이즈가 틀어질 수 있으므로 복구 로직 추가
            if (_backgroundRenderer.drawMode != SpriteDrawMode.Simple)
            {
                Vector2 size = _backgroundRenderer.size;
                _backgroundRenderer.sprite = _originalSprite;
                _backgroundRenderer.size = size;
            }
            else
            {
                _backgroundRenderer.sprite = _originalSprite;
            }

            Color color = _backgroundRenderer.color;
            color.a = 1f;
            _backgroundRenderer.color = color;
        }
    }

    private IEnumerator ProcessCrossfade(Sprite targetSprite)
    {
        // 1. 원본 오브젝트를 통째로 복제 
        // Transform(위치, 회전, 스케일)과 컴포넌트 설정이 모두 복사됨
        _tempObject = Instantiate(_backgroundRenderer.gameObject, _backgroundRenderer.transform.position, _backgroundRenderer.transform.rotation, _backgroundRenderer.transform.parent);
        _tempObject.name = "TempBackground";

        // 2. 복제된 오브젝트에서 불필요한 컴포넌트(스크립트 등) 제거하여 충돌 방지
        var script = _tempObject.GetComponent<StageBackgroundChanger>();
        if (script != null) Destroy(script);

        // 3. 임시 렌더러 설정
        SpriteRenderer tempRenderer = _tempObject.GetComponent<SpriteRenderer>();

        Vector2 originalSize = _backgroundRenderer.size;
        SpriteDrawMode originalMode = _backgroundRenderer.drawMode;

        // 4. 스프라이트 교체 및 속성 강제 재적용
        tempRenderer.sprite = targetSprite;

        // Sliced나 Tiled 모드일 경우 사이즈가 중요하므로 다시 적용
        if (originalMode != SpriteDrawMode.Simple)
        {
            tempRenderer.drawMode = originalMode; // 모드 재확인
            tempRenderer.size = originalSize;     // 사이즈 강제 복구
        }

        // 기존 배경보다 한 단계 앞에서 그려지도록 설정
        tempRenderer.sortingOrder = _backgroundRenderer.sortingOrder + 1;

        // 5. 페이드 인 (투명 -> 불투명)
        Color targetColor = _backgroundRenderer.color;
        tempRenderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);

        float timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / _fadeDuration);
            tempRenderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
            yield return null;
        }

        // 6. 완료 처리: 원본 렌더러 교체
        if (_backgroundRenderer != null)
        {
            // 원본도 교체 시 사이즈가 초기화될 수 있으므로 동일하게 처리
            Vector2 currentSize = _backgroundRenderer.size;
            SpriteDrawMode currentMode = _backgroundRenderer.drawMode;

            _backgroundRenderer.sprite = targetSprite;

            if (currentMode != SpriteDrawMode.Simple)
            {
                _backgroundRenderer.drawMode = currentMode;
                _backgroundRenderer.size = currentSize;
            }
        }

        if (_tempObject != null)
        {
            Destroy(_tempObject);
            _tempObject = null;
        }
    }
}