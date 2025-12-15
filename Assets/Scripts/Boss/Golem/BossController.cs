using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    // 보스 상태 정의
    public enum BossState { Intro, Idle, ScrapPattern, ConcretePattern, WeaknessPattern, Die }

    [Header("State")]
    [SerializeField] private BossState _currentBossState = BossState.Intro;

    [Header("Boss Models")]
    [SerializeField] private Transform _sideBossModel;   // 벽에서 나오는 보스
    [SerializeField] private Transform _bottomBossModel; // 바닥에서 나오는 보스

    [Header("Positions")]
    [SerializeField] private Transform _sideSpawnPoint;     // 맵 우측 벽 가운데 (등장 위치)
    [SerializeField] private Transform _sideRetreatPoint;   // 맵 밖 우측 (퇴장 위치)
    [SerializeField] private Transform _bottomRetreatPoint; // 맵 밖 아래 (퇴장 위치)

    [Header("References")]
    [SerializeField] private Transform _playerTransform;    // 플레이어 위치 추적용

    [Header("Settings")]
    [SerializeField] private float _bossScrapPatternSpeed = 0.5f;    // 고철 패턴 이동 속도
    [SerializeField] private float _bossConcretePatternSpeed = 1.0f; // 콘크리트 패턴 이동 속도

    [Header("Manager")]
    [SerializeField] private BossObjectManager _bossObjectManager;   // 투사체 생성 매니저 연결

    private void Start()
    {
        // 1. 초기화: 보스들을 화면 밖으로 이동
        _sideBossModel.position = _sideRetreatPoint.position;
        _bottomBossModel.position = _bottomRetreatPoint.position;

        // 2. 인트로 시작
        StartCoroutine(IntroProcess());
    }

    // ====================================================
    // 1. Intro (등장 연출)
    // ====================================================
    private IEnumerator IntroProcess()
    {
        yield return new WaitForSeconds(2.0f);

        // 우측 벽 가운데로 이동 (등장)
        yield return StartCoroutine(MoveOverTime(_sideBossModel, _sideSpawnPoint.position, 0.5f));

        yield return new WaitForSeconds(2.0f);

        // 맵 밖으로 퇴장
        yield return StartCoroutine(MoveOverTime(_sideBossModel, _sideRetreatPoint.position, 0.5f));

        // 고철 던지기 패턴으로 상태 변경
        ChangeState(BossState.ScrapPattern);
    }

    // 상태 변경 및 패턴 실행 분기점
    private void ChangeState(BossState newState)
    {
        _currentBossState = newState;

        switch (_currentBossState)
        {
            case BossState.ScrapPattern:
                StartCoroutine(ScrapPatternProcess());
                break;

            case BossState.ConcretePattern:
                StartCoroutine(ConcretePatternProcess());
                break;

            case BossState.WeaknessPattern:
                StartCoroutine(WeaknessPatternProcess());
                break;

            default:
                Debug.Log($"정의되지 않은 보스 상태입니다: {_currentBossState}");
                break;
        }
    }

    // ====================================================
    // 2. 고철 던지기 패턴 (Scrap Metal)
    // ====================================================
    private IEnumerator ScrapPatternProcess()
    {
        // 1. 플레이어 Y축을 추적하여 등장 위치 계산
        Vector3 targetPos = new Vector3(_sideSpawnPoint.position.x, _playerTransform.position.y, 0);

        // 0.5초에 걸쳐 등장
        yield return StartCoroutine(MoveOverTime(_sideBossModel, targetPos, 0.5f));

        // 1.5초 대기
        yield return new WaitForSeconds(1.5f);

        // 2. 고철 공격 오브젝트 생성
        _bossObjectManager.SpawnScrap(_sideBossModel.position);

        yield return new WaitForSeconds(0.5f);

        // 3. 0.5초 동안 맵 밖으로 퇴장
        yield return StartCoroutine(MoveOverTime(_sideBossModel, _sideRetreatPoint.position, 0.5f));

        // 다음 패턴(콘크리트)으로 전환
        ChangeState(BossState.ConcretePattern);
    }

    // ====================================================
    // 3. 콘크리트 스킬 패턴
    // ====================================================
    private IEnumerator ConcretePatternProcess()
    {
        // --- 1단계: 벽 보스 공격 (가로) ---
        yield return StartCoroutine(PerformSideAttack());

        yield return new WaitForSeconds(2.0f);

        // --- 2단계: 바닥 보스 공격 (세로) ---
        yield return StartCoroutine(PerformBottomAttack());

        yield return new WaitForSeconds(2.0f);

        // --- 3단계: 벽 보스 재공격 (가로) ---
        yield return StartCoroutine(PerformSideAttack());

        // 패턴 종료 후 잠시 대기했다가 약점 패턴으로
        yield return new WaitForSeconds(2.0f);
        ChangeState(BossState.WeaknessPattern);
    }

    // 벽 보스 공격 로직 분리 (재사용을 위해)
    private IEnumerator PerformSideAttack()
    {
        // 플레이어 Y축으로 이동
        Vector3 targetPos = new Vector3(_sideSpawnPoint.position.x, _playerTransform.position.y, 0);
        yield return StartCoroutine(MoveOverTime(_sideBossModel, targetPos, _bossConcretePatternSpeed));

        yield return new WaitForSeconds(2.0f);

        // 가로 콘크리트 생성 (true)
        _bossObjectManager.SpawnConcrete(_sideBossModel.position, true);

        yield return new WaitForSeconds(0.5f);

        // 퇴장
        yield return StartCoroutine(MoveOverTime(_sideBossModel, _sideRetreatPoint.position, 0.5f));
    }

    // 바닥 보스 공격 로직 분리
    private IEnumerator PerformBottomAttack()
    {
        // 플레이어 X축, 바닥 Y좌표(-3.5f)로 이동
        Vector3 targetPos = new Vector3(_playerTransform.position.x, -3.5f, 0);
        yield return StartCoroutine(MoveOverTime(_bottomBossModel, targetPos, _bossConcretePatternSpeed));

        yield return new WaitForSeconds(2.0f);

        // 세로 콘크리트 생성 (false)
        _bossObjectManager.SpawnConcrete(_bottomBossModel.position, false);

        yield return new WaitForSeconds(0.5f);

        // 퇴장
        yield return StartCoroutine(MoveOverTime(_bottomBossModel, _bottomRetreatPoint.position, 0.5f));
    }

    // ====================================================
    // 4. 약점 노출 패턴
    // ====================================================
    private IEnumerator WeaknessPatternProcess()
    {
        // 맵 중앙 아래 약점 오브젝트 생성
        GameObject weakness = _bossObjectManager.SpawnWeakness(new Vector3(0, -2, 0));

        float timer = 0;
        float duration = 5.0f; // 5초 동안 유지

        while (timer < duration)
        {
            // TODO: 만약 보스가 죽었다면(IsGameClear)에서 중단해야 함
            // if (BossStatusManager.Instance.IsGameClear) yield break; 

            timer += Time.deltaTime;
            yield return null;
        }

        // 시간이 다 되면 약점 제거
        if (weakness != null)
        {
            Destroy(weakness);
        }

        // 보스가 아직 살아있다면 다시 콘크리트 패턴 반복
        ChangeState(BossState.ConcretePattern);
    }

    // ====================================================
    // 유틸리티: 부드러운 이동
    // ====================================================
    private IEnumerator MoveOverTime(Transform targetObj, Vector3 destination, float duration)
    {
        Vector3 startPos = targetObj.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            // 시간에 따른 선형 보간 이동
            targetObj.position = Vector3.Lerp(startPos, destination, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // 오차 없이 정확한 위치로 강제 이동
        targetObj.position = destination;
    }
}