using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

// 인스펙터에서 수정 가능하도록 Serializable 추가
[System.Serializable]
public class BossData
{
    [Header("=== 1. 보스 기본 스탯 ===")]
    [Tooltip("보스 최대 체력")]
    public float maxHp = 100;

    [Tooltip("몸체 충돌 데미지")]
    public float bodyContactDamage = 100f;


    [Header("=== 2. 페이즈 설정 ===")]
    [Tooltip("보스방 입장 후 등장 전까지 대기 시간")]
    public float introDelay = 3.0f;

    [Tooltip("2페이즈로 넘어가는 체력 기준")]
    public float phase2HpThreshold = 60f;

    [Tooltip("3페이즈로 넘어가는 체력 기준 ")]
    public float phase3HpThreshold = 40f;

    [Tooltip("페이즈 전환 사이의 쉬는 시간")]
    public float phaseTransitionDelay = 1.0f;


    [Header("=== 3. 고철(Scrap) 패턴 ===")]
    [Tooltip("고철 투사체 이동 속도")]
    public float scrapSpeed = 20f;

    [Tooltip("발사 전 조준하며 뜸 들이는 시간")]
    public float scrapAimDelay = 0.5f;

    [Tooltip("발사 후 잠시 대기하는 시간")]
    public float scrapFireDelay = 0.2f;

    [Tooltip("패턴 한 사이클 끝난 후 다음 발사까지 대기 시간")]
    public float scrapCycleWaitTime = 0.5f;

    [Tooltip("고철 투사체 원본 데미지")]
    public float scrapDamage = 20f;

    [Tooltip("고철이 벽에 부딪혀 쪼개진 파편 데미지")]
    public float scrapFragDamage = 5f;

    [Tooltip("고철/파편에 맞았을 때 넉백")]
    public float scrapKnockback = 5.0f;

    [Tooltip("고철이 날아가는 최대 시간 (지나면 자동 삭제)")]
    public float scrapLifeTime = 5.0f;


    [Header("=== 4. 콘크리트(Concrete) 패턴 ===")]
    [Tooltip("콘크리트가 솟아오르는 시간 (작을수록 빠르게 튀어나옴)")]
    public float concreteMoveTime = 0.3f;

    [Tooltip("연속 공격 시 콘크리트 생성 간격 (작을수록 연사 빨라짐)")]
    public float concreteInterval = 0.3f;

    [Tooltip("패턴 종료 후 콘크리트가 다시 들어가는 시간")]
    public float concreteRetractDuration = 1.0f;

    [Tooltip("콘크리트 충돌 데미지")]
    public float concreteDamage = 10f;

    [Tooltip("콘크리트 충돌 넉백 파워")]
    public float concreteKnockback = 5f;

    [Tooltip("가로 콘크리트 생성 위치 오프셋")]
    public float concreteSpawnOffsetH = -10.0f;

    [Tooltip("세로 콘크리트 생성 위치 오프셋")]
    public float concreteSpawnOffsetV = -10.0f;

    [Tooltip("콘크리트 패턴 후 다음 행동까지 대기 시간")]
    public float patternWaitTime = 1.5f;


    [Header("=== 5. 약점(Weakness) 패턴 ===")]
    [Tooltip("약점이 노출되어 있는 최대 시간")]
    public float weaknessDuration = 5f;

    [Tooltip("약점 공략 성공 시, 다음 행동까지 딜레이")]
    public float weaknessSuccessWaitTime = 2.5f;


    [Header("=== 6. 보스 이동 연출 ===")]
    [Tooltip("보스가 벽에서 나왔다 들어갔다 하는 이동 시간")]
    public float bossMoveDuration = 0.5f;

    [Tooltip("보스가 벽에서 튀어나오는 거리")]
    public float bossAppearDistance = 4.0f;
}

public class BossController : MonoBehaviour
{
    [Header("보스 데이터 설정")]
    [SerializeField] 
    private BossData _data = new BossData();
    public BossData Data => _data; // 외부에서는 읽기만 가능

    [Header("연결 (References)")]
    public Transform player;
    public Transform centerPoint;

    [Header("보스 오브젝트")]
    public Transform wallBossObject;
    public Transform floorBossObject;

    [Header("프리팹")]
    public GameObject scrapPrefab;
    public GameObject concreteHPrefab;
    public GameObject concreteVPrefab;
    public GameObject weaknessObject;

    [SerializeField]
    private float _currentHp;

    private BossState _currentState;
    public float CurrentHp
    {
        get => _currentHp;
        private set => _currentHp = value;
    }

    // 현재 나와있는 콘크리트 오브젝트 리스트
     private List<ConcreteObject> _activeConcretes = new List<ConcreteObject>();

    public int Phase { get; private set; } = 1;


    private bool _isActivated = false;

    private void Start()
    {

        CurrentHp = Data.maxHp;
        if (weaknessObject != null)
        {
            weaknessObject.SetActive(false);
        }
        if (wallBossObject != null)
        {
            wallBossObject.gameObject.SetActive(false);
        }
        if (floorBossObject != null)
        {
            floorBossObject.gameObject.SetActive(false);
        }
    }

    public void ActivateBoss()
    {
        if (_isActivated) return; // 이미 시작했으면 무시
        _isActivated = true;


        Debug.Log("플레이어 입장. 보스전 시작");

        // 패턴 루틴 시작
        StartCoroutine(MainFlowRoutine());
    }

    private IEnumerator MainFlowRoutine()
    {

        yield return new WaitForSeconds(Data.introDelay);

        // === [인트로] ===
        Debug.Log("인트로 시작");
        yield return new WaitForSeconds(Data.patternWaitTime);

        Vector3 hiddenPos = wallBossObject.position;
        Vector3 appearPos = hiddenPos + Vector3.left * Data.bossAppearDistance;

        if (wallBossObject != null) wallBossObject.gameObject.SetActive(true);
        if (floorBossObject != null) floorBossObject.gameObject.SetActive(true);

        yield return StartCoroutine(MoveBossTo(wallBossObject, appearPos, Data.bossMoveDuration));
        yield return new WaitForSeconds(Data.patternWaitTime);
        yield return StartCoroutine(MoveBossTo(wallBossObject, hiddenPos, Data.bossMoveDuration));

        Debug.Log("인트로 종료 -> 1페이즈 시작");

        // [Phase 1]
        yield return ChangeState(new BossScrapState(this, 3));

        CurrentHp = Data.phase2HpThreshold;

        yield return new WaitForSeconds(Data.phaseTransitionDelay);
        Phase = 2;

        // [Phase 2 Loop]
        while (CurrentHp > Data.phase3HpThreshold)
        {
            yield return ChangeState(new BossConcreteState(this, 3, true));
            yield return ChangeState(new BossWeaknessState(this));
        }

        Phase = 3;
        // [Phase 3 Loop]
        while (CurrentHp > 0)
        {

            yield return ChangeState(new BossConcreteState(this, 1, false));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 2, false));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 2, false));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 1, true));
            yield return ChangeState(new BossWeaknessState(this));
        }

        Die();
    }

    //콘크리트 오브젝트 등록
    public void RegisterConcrete(ConcreteObject concrete)
    {
        _activeConcretes.Add(concrete);
    }

    //일괄 회수 
    public void RetractAllConcretes()
    {
        foreach (var concrete in _activeConcretes)
        {
            if (concrete != null)
                concrete.StartRetract();
        }
        _activeConcretes.Clear(); // 명단 초기화
    }

    private IEnumerator ChangeState(BossState newState)
    {
        if (_currentState != null) _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
        yield return StartCoroutine(_currentState.Execute());
    }

    public void TakeDamage(float damage)
    {
        if (weaknessObject.activeSelf)
        {
            CurrentHp -= damage;
            Debug.Log($"보스 HP: {CurrentHp}");
            if (CurrentHp <= 0) Die();
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        if (wallBossObject) wallBossObject.gameObject.SetActive(false);
        if (floorBossObject) floorBossObject.gameObject.SetActive(false);
        Debug.Log("게임 클리어");
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
}



