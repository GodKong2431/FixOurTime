using System.Collections;
using UnityEngine;

public class Stage3AngelBoss : BossBase
{
    [Header("Gimmicks")]
    [SerializeField] private Stage3AngelPlatform[] _platforms; // 발판들
    [SerializeField] private FeatherSpawner _featherSpawner;   // 깃털 스포너
    [SerializeField] private Angel _angelPattern;              // 천사 패턴 컨트롤러
    [SerializeField] private Judgment _judgment;      // Judgment 스크립트 연결

    [Header("Next Phase")]
    [SerializeField] private Stage3DevilBoss _devilBoss;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _coreRenderer;

    private bool _isActivated = false;

    // 보스 활성화 시 호출됨
    public override void ActivateBoss()
    {

        if (_isActivated) return;
        _isActivated = true;

        Debug.Log("천사 보스전 시작");

        // 1. 깃털 패턴 시작
        if (_featherSpawner != null)
            _featherSpawner.StartSpawn();
    }

    // 코어(AngelCore)에서 호출할 데미지 함수
    public void ApplyDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public override void ResetBoss()
    {
        // 1. 기본 리셋
        base.ResetBoss();

        _isActivated = false;

        // 2. 기믹들 초기화
        if (_featherSpawner != null) _featherSpawner.StopSpawn();
        if (_judgment != null) _judgment.StopJudgment();
        if (_angelPattern != null) _angelPattern.StopCheck();

        // 3. 발판 다시 불러오기
        if (_platforms != null)
        {
            foreach (var platform in _platforms)
            {
                if (platform != null) platform.ResetPlatform();
            }
        }

        if (_coreRenderer != null)
        {
            Color c = _coreRenderer.color; c.a = 1f; _coreRenderer.color = c;
        }

        gameObject.SetActive(false);

        Debug.Log("천사 보스 리셋 완료 (1페이즈 대기 상태)");
    }

    protected override void Die()
    {
        Debug.Log("천사 보스 사망");

        StopAllPatterns();

        // 2. 연출 코루틴 시작
        StartCoroutine(DeathSequenceCoroutine());

    }

    private void StopAllPatterns()
    {
        if (_featherSpawner != null) _featherSpawner.StopSpawn();
        if (_judgment != null) _judgment.StopJudgment();
        if (_angelPattern != null) _angelPattern.StopCheck();

        
    }

    private IEnumerator DeathSequenceCoroutine()
    {
        // ------------------------------------------------
        // 1단계: 흔들림 (Shaking)
        // ------------------------------------------------
        float shakeDuration = 1.5f; // 흔들리는 시간
        float shakeMagnitude = 0.2f; // 흔들리는 강도
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // 랜덤한 위치로 떨림 효과
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos; // 위치 복구

        // 1. 천사 발판 퇴장 시작
        if (_platforms != null)
        {
            foreach (var platform in _platforms)
            {
                if (platform != null) platform.OutPlatform();
            }
        }

        // 2. 악마 발판 진입 시작 (미리 소환)
        if (_devilBoss != null)
        {
            if (!_devilBoss.gameObject.activeSelf) _devilBoss.gameObject.SetActive(true);
            _devilBoss.SetAlpha(0f);
            _devilBoss.SummonTiles(); // 타일 이동 시작
        }

        // ------------------------------------------------
        // 2단계: 크로스 페이드 (Cross-Fade)
        // ------------------------------------------------
        if (_devilBoss != null)
        {
            // 악마 보스 오브젝트 켜기 
            if (!_devilBoss.gameObject.activeSelf)
                _devilBoss.gameObject.SetActive(true);

            // 악마를 완전히 투명하게 설정하고 시작
            _devilBoss.SetAlpha(0f);
        }

        float fadeDuration = 2.0f; // 페이드 시간
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;

            // 천사: 1 -> 0 (사라짐)
            if (_renderer != null)
            {
                Color c = _renderer.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                _renderer.color = c;
            }
            //  코어도 같이 투명화
            if (_coreRenderer != null)
            {
                Color c = _coreRenderer.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                _coreRenderer.color = c;
            }

            // 악마: 0 -> 1 (나타남)
            if (_devilBoss != null)
            {
                _devilBoss.SetAlpha(Mathf.Lerp(0f, 1f, t));
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 확실하게 값 고정
        if (_renderer != null)
        {
            Color c = _renderer.color; c.a = 1f; _renderer.color = c;
        }
        if (_devilBoss != null) _devilBoss.SetAlpha(1f);


        // ------------------------------------------------
        // 3단계: 마무리 및 악마 전투 시작
        // ------------------------------------------------

        // 천사 퇴장
        gameObject.SetActive(false);

        // 악마 전투 시작
        if (_devilBoss != null)
        {
            _devilBoss.ActivateBoss();
        }
    }
}