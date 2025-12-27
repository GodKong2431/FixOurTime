using UnityEngine;
using System.Collections;

public class Stage3DevilBoss : BossBase
{

    [Header("Stage3 Devil Data")]
    [SerializeField] private Boss3DevilData _bossData = new Boss3DevilData();

    [Header("Phase 2 Objects")]
    [SerializeField] private DevilTile[] _devilTiles;
    public Boss3DevilData Data => _bossData;

    [Header("Devil Pattern Controller")]
    [SerializeField] private DevilPatternController _patternController;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _coreRenderer;
    private bool _isActivated = false;

    protected override void Start()
    {
        base.Start();
        if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();

        // 데이터에서 MaxHp 읽어서 보스 체력 세팅
        if (_bossData != null)
        {
            _bossMaxHp = _bossData.BossMaxHp;
        }

        base.Start();

        if (_patternController == null)
        {
            _patternController = GetComponent<DevilPatternController>();
        }

        // 패턴 컨트롤러가 데이터에 접근할 수 있게 초기화
        if (_patternController != null)
        {
            _patternController.Initialize(this);
        }
    }

    public void SetAlpha(float alpha)
    {
        if (_renderer != null)
        {
            Color color = _renderer.color;
            color.a = alpha;
            _renderer.color = color;
        }
        // 코어 투명도 조절 
        if (_coreRenderer != null)
        {
            Color color = _coreRenderer.color;
            color.a = alpha;
            _coreRenderer.color = color;
        }
    }
    /// <summary>
    /// BossZone에서 호출해서 보스를 활성화하는 진입점
    /// </summary>
    public override void ActivateBoss()
    {
        if (_isActivated) return;
        _isActivated = true;

        Debug.Log("[Stage3DevilBoss] 악마 보스 활성화");

        // 패턴 시작
        if (_patternController != null)
        {
            _patternController.StartPattern();
        }
    }

    public override void ResetBoss()
    {
        // 1. 기본 리셋
        base.ResetBoss();

        _isActivated = false;

        // 2. 패턴 중지
        if (_patternController != null)
        {
            _patternController.StopPattern(); // 코루틴 정지

            // 악마 손 컨트롤러를 가져와서 강제 복귀 
            var handController = _patternController.GetComponent<DevilHandController>();
            if (handController != null)
            {
                handController.ForceResetHands(); // OnDisable 대신 여기서 직접 호출
            }

            // 창(DarkSpear)도 리셋 
            var spearController = _patternController.GetComponent<DevilDarkSpearController>();
            if (spearController != null)
            {
                spearController.StopController();
            }
            var blackHoleController = _patternController.GetComponent<DevilBlackHoleController>();
            if (blackHoleController != null)
            {
                blackHoleController.StopBlackHole();
            }
        }

        // 3. 타일 원위치 시키기
        if (_devilTiles != null)
        {
            foreach (var tile in _devilTiles)
            {
                if (tile != null) tile.ResetTile();
            }
        }

        gameObject.SetActive(false);

        Debug.Log("[Stage3DevilBoss] 악마 보스 리셋 완료");
    }

    /// <summary>
    /// DevilCore에서 호출: 보스 HP 감소 + 디버그 로그
    /// </summary>
    public void ApplyDamage(float damage, float knockbackForce, Vector3 hitPos)
    {
        if (!_isActivated) return;

        base.TakeDamage(damage);
    }
   

    protected override void Die()
    {
        Debug.Log("[Stage3DevilBoss] 악마 보스 사망");

        if (_patternController != null)
        {
            _patternController.StopPattern();

            var handController = _patternController.GetComponent<DevilHandController>();
            if (handController != null)
            {
                handController.ForceResetHands();
            }

            var spearController = _patternController.GetComponent<DevilDarkSpearController>();
            if (spearController != null)
            {
                spearController.StopController();
            }
            var blackHoleController = _patternController.GetComponent<DevilBlackHoleController>();
            if (blackHoleController != null)
            {
                blackHoleController.StopBlackHole();
            }
        }

        StartCoroutine(DeathSequenceCoroutine());
    }

    private IEnumerator DeathSequenceCoroutine()
    {
        // ------------------------------------------------
        // 1단계: 흔들림 (Shaking) - 천사와 동일하게
        // ------------------------------------------------
        float shakeDuration = 1.5f;   // 흔들리는 시간
        float shakeMagnitude = 0.2f;  // 흔들리는 강도
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // 랜덤 위치 떨림
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos; // 위치 복구

        // ------------------------------------------------
        // 2단계: 페이드 아웃 (Fade Out)
        // ------------------------------------------------
        float fadeDuration = 2.0f;
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;

            // 1 -> 0 으로 투명해짐 (본체 + 코어)
            SetAlpha(Mathf.Lerp(1f, 0f, t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 완전히 투명하게 고정
        SetAlpha(0f);

        // ------------------------------------------------
        // 3단계: 비활성화 (완전 종료)
        // ------------------------------------------------
        gameObject.SetActive(false);
    }
    public void SummonTiles()
    {
        if (_devilTiles != null)
        {
            foreach (var tile in _devilTiles)
            {
                if (tile != null) tile.Move();
            }
        }
    }
}