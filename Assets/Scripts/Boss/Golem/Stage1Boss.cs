using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

// 인스펙터에서 수정 가능하도록 Serializable 추가
[System.Serializable]
public class Boss1Data : BossCommonData
{
    [Tooltip("몸체 충돌 데미지")]
    [SerializeField] private float _bossBodyContactDamage = 100f;

    [Header("Phase Settings")]
    [Tooltip("보스방 입장 후 등장 전까지 대기 시간")]
    [SerializeField] private float _introDelay = 3.0f;

    [Tooltip("2페이즈로 넘어가는 체력 기준")]
    [SerializeField] private float _phase2HpThreshold = 60f;

    [Tooltip("3페이즈로 넘어가는 체력 기준 ")]
    [SerializeField] private float _phase3HpThreshold = 40f;

    [Tooltip("페이즈 전환 사이의 쉬는 시간")]
    [SerializeField] private float _phaseTransitionDelay = 1.0f;

    [Tooltip("패턴 후 다음 행동까지 대기 시간")]
    [SerializeField] private float _patternWaitTime = 1.5f;


    [Header("Scrap Pattern")]
    [Tooltip("고철 투사체 이동 속도")]
    [SerializeField] private float _scrapSpeed = 20f;

    [Tooltip("발사 전 조준하며 뜸 들이는 시간")]
    [SerializeField] private float _scrapAimDelay = 0.5f;

    [Tooltip("발사 후 잠시 대기하는 시간")]
    [SerializeField] private float _scrapFireDelay = 0.2f;

    [Tooltip("패턴 한 사이클 끝난 후 다음 발사까지 대기 시간")]
    [SerializeField] private float _scrapCycleWaitTime = 0.5f;

    [Tooltip("고철 투사체 원본 데미지")]
    [SerializeField] private float _scrapDamage = 20f;

    [Tooltip("고철이 벽에 부딪혀 쪼개진 파편 데미지")]
    [SerializeField] private float _scrapFragDamage = 5f;

    [Tooltip("고철/파편에 맞았을 때 넉백")]
    [SerializeField] private float _scrapKnockback = 5.0f;

    [Tooltip("고철이 날아가는 최대 시간 (지나면 자동 삭제)")]
    [SerializeField] private float _scrapLifeTime = 5.0f;


    [Header("Concrete Pattern")]
    [Tooltip("콘크리트가 솟아오르는 시간 (작을수록 빠르게 튀어나옴)")]
    [SerializeField] private float _concreteMoveTime = 0.3f;

    [Tooltip("연속 공격 시 콘크리트 생성 간격 (작을수록 연사 빨라짐)")]
    [SerializeField] private float _concreteInterval = 0.3f;

    [Tooltip("패턴 종료 후 콘크리트가 다시 들어가는 시간")]
    [SerializeField] private float _concreteRetractDuration = 1.0f;

    [Tooltip("콘크리트 앞면에 맞았을 때 데미지")]
    [SerializeField] private float _concreteDamage = 10f;

    [Tooltip("콘크리트 옆면에 스쳤을 때 데미지")]
    [SerializeField] private float _concreteSideDamage = 2f;

    [Tooltip("콘크리트 충돌 넉백 파워")]
    [SerializeField] private float _concreteKnockback = 5f;

    [Tooltip("가로 콘크리트 생성 위치 오프셋")]
    [SerializeField] private float _concreteSpawnOffsetH = -10.0f;

    [Tooltip("세로 콘크리트 생성 위치 오프셋")]
    [SerializeField] private float _concreteSpawnOffsetV = -10.0f;


    [Header("Weakness Pattern")]
    [Tooltip("약점이 노출되어 있는 최대 시간")]
    [SerializeField] private float _weaknessDuration = 5f;

    [Tooltip("약점 공략 성공 시, 다음 행동까지 딜레이")]
    [SerializeField] private float _weaknessSuccessWaitTime = 2.5f;


    [Header("Movement")]
    [Tooltip("보스가 벽에서 나왔다 들어갔다 하는 이동 시간")]
    [SerializeField] private float _bossMoveDuration = 0.5f;

    [Tooltip("보스가 벽에서 튀어나오는 거리")]
    [SerializeField] private float _bossAppearDistance = 4.0f;


    // === 프로퍼티 (외부 접근용) ===

    public float BossBodyContactDamage => _bossBodyContactDamage;
    public float IntroDelay => _introDelay;
    public float Phase2HpThreshold => _phase2HpThreshold;
    public float Phase3HpThreshold => _phase3HpThreshold;
    public float PhaseTransitionDelay => _phaseTransitionDelay;
    public float PatternWaitTime => _patternWaitTime;

    public float ScrapSpeed => _scrapSpeed;
    public float ScrapAimDelay => _scrapAimDelay;
    public float ScrapFireDelay => _scrapFireDelay;
    public float ScrapCycleWaitTime => _scrapCycleWaitTime;
    public float ScrapDamage => _scrapDamage;
    public float ScrapFragDamage => _scrapFragDamage;
    public float ScrapKnockback => _scrapKnockback;
    public float ScrapLifeTime => _scrapLifeTime;

    public float ConcreteMoveTime => _concreteMoveTime;
    public float ConcreteInterval => _concreteInterval;
    public float ConcreteRetractDuration => _concreteRetractDuration;
    public float ConcreteDamage => _concreteDamage;
    public float ConcreteSideDamage => _concreteSideDamage;
    public float ConcreteKnockback => _concreteKnockback;
    public float ConcreteSpawnOffsetH => _concreteSpawnOffsetH;
    public float ConcreteSpawnOffsetV => _concreteSpawnOffsetV;

    public float WeaknessDuration => _weaknessDuration;
    public float WeaknessSuccessWaitTime => _weaknessSuccessWaitTime;

    public float BossMoveDuration => _bossMoveDuration;
    public float BossAppearDistance => _bossAppearDistance;
}

public class Stage1Boss : BossBase
{
    [Header("Stage1 Data")]
    [SerializeField] private Boss1Data _bossData = new Boss1Data();
    public Boss1Data BossData => _bossData;

    [Header("Objects")]
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private Transform _wallBossObject;
    [SerializeField] private Transform _floorBossObject;
    [SerializeField] private GameObject _weaknessObject;

    [Header("Prefabs")]
    [SerializeField] private GameObject _scrapPrefab;
    [SerializeField] private GameObject _concreteHPrefab;
    [SerializeField] private GameObject _concreteVPrefab;

    private List<ConcreteObject> _activeConcretes = new List<ConcreteObject>();
    private bool _isActivated = false;
    public int Phase { get; private set; } = 1;

    private Vector3 _originWallPos;
    private Vector3 _originFloorPos;

    // 외부(State) 접근용 프로퍼티
    public Transform CenterPoint => _centerPoint;
    public Transform WallBossObject => _wallBossObject;
    public Transform FloorBossObject => _floorBossObject;
    public GameObject WeaknessObject => _weaknessObject;
    public GameObject ScrapPrefab => _scrapPrefab;
    public GameObject ConcreteHPrefab => _concreteHPrefab;
    public GameObject ConcreteVPrefab => _concreteVPrefab;
    public Transform PlayerTarget => _playerTarget; // 부모의 protected 필드 공개

    protected override void Start()
    {
        _bossMaxHp = _bossData.BossMaxHp;
        base.Start();

        if (_wallBossObject) _originWallPos = _wallBossObject.position;
        if (_floorBossObject) _originFloorPos = _floorBossObject.position;

        // 초기화
        if (_weaknessObject) _weaknessObject.SetActive(false);
        if (_wallBossObject) _wallBossObject.gameObject.SetActive(false);
        if (_floorBossObject) _floorBossObject.gameObject.SetActive(false);
    }

    public override void ActivateBoss()
    {
        if (_isActivated) return;

        _isActivated = true;
        Debug.Log("Stage 1 보스전 시작");
        StartCoroutine(MainFlowRoutine());
    }

    public IEnumerator MoveBossTo(Transform bossTr, Vector3 targetPos, float duration)
    {
        Vector3 start = bossTr.position;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            bossTr.position = Vector3.Lerp(start, targetPos, t / duration);
            yield return null;
        }
        bossTr.position = targetPos;
    }

    private IEnumerator MainFlowRoutine()
    {
        yield return new WaitForSeconds(_bossData.IntroDelay);

        // === 인트로 ===
        Vector3 hiddenPos = _wallBossObject.position;
        Vector3 appearPos = hiddenPos + Vector3.left * _bossData.BossAppearDistance;

        if (_wallBossObject) _wallBossObject.gameObject.SetActive(true);
        if (_floorBossObject) _floorBossObject.gameObject.SetActive(true);

        yield return StartCoroutine(MoveBossTo(_wallBossObject, appearPos, _bossData.BossMoveDuration));
        yield return new WaitForSeconds(_bossData.PatternWaitTime);
        yield return StartCoroutine(MoveBossTo(_wallBossObject, hiddenPos, _bossData.BossMoveDuration));

        // === Phase 1 ===
        yield return ChangeState(new BossScrapState(this, 3));

        // 1페이즈 루프: 체력이 60% 이하가 될 때까지 [콘크리트 3회 -> 약점] 반복
        while (_currentHp > _bossData.Phase2HpThreshold)
        {
            Debug.Log("Phase 1 반복 시작. 현재 HP: " + _currentHp);
            // 콘크리트 3회 (회수 포함 true)
            yield return ChangeState(new BossConcreteState(this, 3, true));
            // 약점 노출
            yield return ChangeState(new BossWeaknessState(this));
        }
        Phase = 2;

        // === Phase 2 Loop ===
        Phase = 2;
        yield return new WaitForSeconds(_bossData.PhaseTransitionDelay);

        // 2페이즈 루프: 체력이 40% 이하가 될 때까지
       
        while (_currentHp > _bossData.Phase3HpThreshold)
        {
            Debug.Log("Phase 2 반복 시작. 현재 HP: " + _currentHp);
            // 콘크리트 5회
            yield return ChangeState(new BossConcreteState(this, 5, true));
            // 약점 노출
            yield return ChangeState(new BossWeaknessState(this));
        }
        Phase = 3;
        // === Phase 3 Loop ===
        while (_currentHp > 0)
        {
            Debug.Log("Phase 3 반복 시작. 현재 HP: " + _currentHp);
            yield return ChangeState(new BossConcreteState(this, 1, false));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 2, false));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 2, false));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 1, true));
            yield return ChangeState(new BossWeaknessState(this));
        }
    }

    public void RegisterConcrete(ConcreteObject concrete)
    {
        _activeConcretes.Add(concrete);
    }

    public void RetractAllConcretes()
    {
        foreach (var concrete in _activeConcretes)
        {
            if (concrete != null)
            {
                concrete.StartRetract();
            }
        }
        _activeConcretes.Clear();
    }

    public override void ResetBoss()
    {
        base.ResetBoss(); // 부모의 HP 리셋, 코루틴 정지 실행

        // === 1스테이지 전용 초기화 (원본 로직 복구) ===

        // 1. 위치 원위치 및 비활성화
        if (_wallBossObject)
        {
            _wallBossObject.gameObject.SetActive(false);
            _wallBossObject.position = _originWallPos;
        }
        if (_floorBossObject)
        {
            _floorBossObject.gameObject.SetActive(false);
            _floorBossObject.position = _originFloorPos;
        }
        if (_weaknessObject)
        {
            _weaknessObject.SetActive(false);
        }

        // 2. 변수 초기화
        Phase = 1;
        _isActivated = false;

        // 3. 맵에 깔린 콘크리트 제거
        RetractAllConcretes();

        Debug.Log("Stage 1 보스 리셋 완료");
    }

    protected override void Die()
    {
        StopAllCoroutines();
        if (_wallBossObject) _wallBossObject.gameObject.SetActive(false);
        if (_floorBossObject) _floorBossObject.gameObject.SetActive(false);

        Debug.Log("Stage 1 보스 클리어!");
    }
}



