using UnityEngine;
using System.Collections;

// 인스펙터에서 수정 가능하도록 Serializable 추가
[System.Serializable]
public class BossData
{
    [Header("기본 스탯")]
    public float maxHp = 100;

    [Header("고철 패턴 설정")]
    public float scrapSpeed = 10;
    public float scrapDamage = 20;
    public float scrapFragDamage = 5;

    [Header("콘크리트 패턴 설정")]
    public float concreteDuration = 5;   // 벽으로 유지되는 시간
    public float concreteMoveTime = 0.5f; // 튀어나오는 속도 (시간)

    [Tooltip("가로 콘크리트 생성 위치 오프셋 (기존 -7.5)")]
    public float concreteSpawnOffsetH = -10.0f; // 화면 밖으로 더 밀기 위해 기본값 수정 제안

    [Tooltip("세로 콘크리트 생성 위치 오프셋 (기존 -7.5)")]
    public float concreteSpawnOffsetV = -10.0f;

    [Header("약점 패턴 설정")]
    public float weaknessDuration = 5;

    [Header("보스 움직임 설정")]
    public float bossMoveDuration = 0.5f; // 보스 등장/퇴장 시간
    public float bossAppearDistance = 4.0f; // 보스가 벽에서 튀어나오는 거리
    public float patternWaitTime = 2.0f; // 패턴 사이 대기 시간
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

        // 보스 모습 드러내기
        if (wallBossObject != null) wallBossObject.gameObject.SetActive(true);
        if (floorBossObject != null) floorBossObject.gameObject.SetActive(true);

        // 패턴 루틴 시작
        StartCoroutine(MainFlowRoutine());
    }

    private IEnumerator MainFlowRoutine()
    {
        // === [인트로] ===
        Debug.Log("인트로 시작");
        yield return new WaitForSeconds(Data.patternWaitTime);

        Vector3 hiddenPos = wallBossObject.position;
        Vector3 appearPos = hiddenPos + Vector3.left * Data.bossAppearDistance;

        yield return StartCoroutine(MoveBossTo(wallBossObject, appearPos, Data.bossMoveDuration));
        yield return new WaitForSeconds(Data.patternWaitTime);
        yield return StartCoroutine(MoveBossTo(wallBossObject, hiddenPos, Data.bossMoveDuration));

        Debug.Log("인트로 종료 -> 1페이즈 시작");

        // [Phase 1]
        yield return ChangeState(new BossScrapState(this, 3));

        CurrentHp = 60; 

        yield return new WaitForSeconds(1.0f);
        Phase = 2;

        // [Phase 2 Loop]
        while (CurrentHp > 40)
        {
            yield return ChangeState(new BossConcreteState(this, 3));
            yield return ChangeState(new BossWeaknessState(this));
        }

        Phase = 3;
        // [Phase 3 Loop]
        while (CurrentHp > 0)
        {
            yield return ChangeState(new BossConcreteState(this, 1));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 2));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 2));
            yield return ChangeState(new BossScrapState(this, 2));
            yield return ChangeState(new BossConcreteState(this, 1));
            yield return ChangeState(new BossWeaknessState(this));
        }

        Die();
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