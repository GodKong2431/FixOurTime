using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// [Stage2Boss.cs]
// 2스테이지 보스 메인 로직 클래스
public class Stage2Boss : BossBase
{
    #region Inspector Fields
    [Header("Stage2 Data")]
    [Tooltip("2스테이지 보스 밸런스 데이터")]
    [SerializeField] private Boss2Data _bossData = new Boss2Data();

    [Header("UI System")]
    [Tooltip("보스 기믹 관리 UI 매니저 (정답 아이템 표시)")]
    [SerializeField] private BossUIManager _uiManager;

    //[Header("Sub-Systems")]
    //[Tooltip("보스전 소환 여우(Fox) 컨트롤러")]
    //[SerializeField] private FoxController _foxController;

    [Tooltip("종이학 생성 위치 배열")]
    [SerializeField] private Transform[] _craneSpawnPoints;

    [Header("Gimmick Settings")]
    [Tooltip("기믹 아이템(정답/함정) 생성 위치 (10곳)")]
    [SerializeField] private Transform[] _itemSpawnPoints;

    [Tooltip("기믹 아이템 스프라이트 리소스 목록")]
    [SerializeField] private Sprite[] _itemSprites;

    [Header("Prefabs")]
    [Tooltip("종이학(Paper Crane) 프리팹")]
    [SerializeField] private GameObject _cranePrefab;

    [Tooltip("전공책(Major Book) 프리팹 (기믹 실패 시 발사)")]
    [SerializeField] private GameObject _majorBookPrefab;

    [Tooltip("기믹 아이템(Gimmick Item) 프리팹")]
    [SerializeField] private GameObject _gimmickItemPrefab;
    #endregion

    #region Private Fields
    // 로직 제어 변수
    private bool _isActivated = false;

    // 기믹 진행 중인지 확인하는 플래그 (자동 리셋 방지용)
    private bool _isGimmickRunning = false;

    // 생성된 오브젝트 관리용 리스트 (리셋 시 제거 위함)
    private List<GimmickItemObject> _activeItems = new List<GimmickItemObject>();
    private List<GameObject> _activeMinions = new List<GameObject>(); // 종이학, 전공책 등

    // 기믹 카운트
    private int _collectedCount = 0;
    private const int TARGET_COUNT = 4; // 정답 개수 상수화

    // 데이터 접근 프로퍼티
    public Boss2Data Data => _bossData;
    public Transform CenterPoint => transform;
    #endregion

    #region Unity Lifecycle
    public override void ActivateBoss()
    {
        if (_isActivated) return;
        _isActivated = true;

        // 시작 시 혹시 남아있는 오브젝트 정리
        ClearStageObjects();

        StartCoroutine(CraneSpawnRoutine());
        StartCoroutine(MainGimmickRoutine());

        // 여우 소환 예약
        Invoke(nameof(SpawnFox), _bossData.FoxSpawnDelay);
    }

    protected override void Die()
    {
        StopAllCoroutines();
        CancelInvoke(); // 예약된 Fox 소환 등 취소

        // 스테이지 오브젝트 전체 클리어 
        ClearStageObjects();

        //  상태 초기화
        _isActivated = false;
        _isGimmickRunning = false;

        Debug.Log("Stage 2 클리어");
    }
    #endregion

    #region Main Logic (Coroutines)
    private IEnumerator CraneSpawnRoutine()
    {
        while (_currentHp > 0)
        {
            yield return new WaitForSeconds(_bossData.CraneSpawnInterval);
            SpawnPaperCrane();
        }
    }

    private IEnumerator MainGimmickRoutine()
    {
        while (_currentHp > 0)
        {
            StartGimmickRound();

            // 단순히 시간만 기다리는 게 아니라, 기믹이 끝날 때(성공/실패)까지 대기
            yield return new WaitUntil(() => _isGimmickRunning == false);

            // 기믹 종료 후 다음 기믹까지 대기 시간 (휴식 텀)
            yield return new WaitForSeconds(_bossData.GimmickInterval);
        }
    }
    #endregion

    #region Helper Methods (Spawn & Gimmick)

    private void SpawnPaperCrane()
    {
        if (_craneSpawnPoints.Length == 0) return;

        int randIdx = Random.Range(0, _craneSpawnPoints.Length);
        GameObject craneObj = Instantiate(_cranePrefab, _craneSpawnPoints[randIdx].position, Quaternion.identity);

        // 미니언 리스트에 추가
        _activeMinions.Add(craneObj);

        //if (craneObj.TryGetComponent(out PaperCraneObject craneScript))
        //{
        //    craneScript.Initialize(this, _playerTarget);
        //}
    }

    private void SpawnFox()
    {
        //if (_foxController != null)
        //{
        //    _foxController.ActivateFox(this, _playerTarget);
        //}
    }

    private void StartGimmickRound()
    {
        Debug.Log(">>> 기믹 시작: 중복 없는 정답 4개 생성");

        // 기믹 시작 플래그 ON
        _isGimmickRunning = true;

        // 1. 상태 초기화
        _collectedCount = 0;
        ClearAllItems(); // 이전 기믹 아이템만 정리

        // 2. 스프라이트 풀 및 정답/함정 선정
        List<Sprite> spritePool = new List<Sprite>(_itemSprites);
        List<Sprite> targetSprites = SelectTargetSprites(spritePool);
        List<Sprite> trapSprites = SelectTrapSprites(spritePool, targetSprites.Count);

        // UI 표시
        if (_uiManager != null)
        {
            _uiManager.ShowTargetItems(targetSprites);
        }

        // 3. 생성 데이터 병합 및 셔플
        List<ItemSetupData> spawnList = new List<ItemSetupData>();
        foreach (var s in targetSprites) spawnList.Add(new ItemSetupData(s, true));
        foreach (var s in trapSprites) spawnList.Add(new ItemSetupData(s, false));

        ShuffleList(spawnList);

        // 4. 오브젝트 배치
        PlaceGimmickItems(spawnList);
    }

    private List<Sprite> SelectTargetSprites(List<Sprite> pool)
    {
        List<Sprite> targets = new List<Sprite>();
        for (int i = 0; i < TARGET_COUNT; i++)
        {
            if (i < pool.Count)
                targets.Add(pool[i]);
            else
                targets.Add(pool[0]); // 예외 처리: 리소스 부족 시 0번 재사용
        }
        return targets;
    }

    private List<Sprite> SelectTrapSprites(List<Sprite> pool, int offset)
    {
        List<Sprite> traps = new List<Sprite>();
        int trapCount = 6; // 함정 개수

        if (pool.Count >= offset + trapCount)
        {
            // 정답 인덱스 이후의 것들을 사용
            for (int i = 0; i < trapCount; i++)
            {
                traps.Add(pool[offset + i]);
            }
        }
        else
        {
            // 리소스 부족 시 랜덤
            for (int i = 0; i < trapCount; i++)
            {
                traps.Add(_itemSprites[Random.Range(0, _itemSprites.Length)]);
            }
        }
        return traps;
    }

    private void PlaceGimmickItems(List<ItemSetupData> spawnList)
    {
        for (int i = 0; i < _itemSpawnPoints.Length; i++)
        {
            if (i >= spawnList.Count) break;

            GameObject obj = Instantiate(_gimmickItemPrefab, _itemSpawnPoints[i].position, Quaternion.identity);
            if (obj.TryGetComponent(out GimmickItemObject script))
            {
                script.Initialize(this, spawnList[i].Sprite, spawnList[i].IsTarget);
                _activeItems.Add(script);
            }
        }
    }

    // 제네릭 셔플 메서드
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    #endregion

    #region Event Handlers (Public)

    // 플레이어가 아이템 획득 시 호출
    public void OnPlayerCollectItem(bool isTarget, Sprite itemSprite = null)
    {
        if (isTarget)
        {
            _collectedCount++;
            Debug.Log($"정답 획득 ({_collectedCount}/{TARGET_COUNT})");

            if (_uiManager != null)
            {
                _uiManager.MarkItemAsCollected(itemSprite);
            }

            if (_collectedCount >= TARGET_COUNT)
            {
                GimmickSuccess();
            }
        }
        else
        {
            Debug.Log("<color=red>오답 획득 (함정)</color>");
            GimmickFail();
        }
    }

    // 여우가 아이템 섭취 시 호출
    public void OnFoxEatItem(bool isTarget)
    {
        if (isTarget)
        {
            Debug.Log("<color=red>여우가 정답 아이템을 먹어버렸습니다</color>");
            GimmickFail();
        }
        else
        {
            Debug.Log("여우가 함정 아이템을 먹었습니다.");
        }
    }
    #endregion

    #region Internal Actions
    private void GimmickSuccess()
    {
        Debug.Log("기믹 성공. 보스 데미지");
        TakeDamage(_bossData.GimmickSuccessDamage);

        ClearAllItems();

        // 기믹 종료 처리
        _isGimmickRunning = false;
    }

    private void GimmickFail()
    {
        Debug.Log("기믹 실패. 전공책 발사");
        ClearAllItems();
        FireMajorBook();

        // 기믹 종료 처리
        _isGimmickRunning = false;
    }

    private void FireMajorBook()
    {
        Vector3 spawnPos = transform.position + Vector3.up * 5;
        GameObject bookObj = Instantiate(_majorBookPrefab, spawnPos, Quaternion.identity);

        //  미니언 리스트에 추가
        _activeMinions.Add(bookObj);

        if (bookObj.TryGetComponent(out MajorBookObject bookScript))
        {
            bookScript.Initialize(this, _playerTarget);
        }
    }

    private void ClearAllItems()
    {
        // 기믹 아이템(책) 정리
        foreach (var item in _activeItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        _activeItems.Clear();

        if (_uiManager != null)
        {
            _uiManager.HideUI();
        }
    }

    // 스테이지 전체 정리 메서드 (리셋용)
    private void ClearStageObjects()
    {
        // 1. 기믹 아이템 정리
        ClearAllItems();

        // 2. 소환된 미니언(종이학, 전공책) 정리
        foreach (var minion in _activeMinions)
        {
            if (minion != null) Destroy(minion);
        }
        _activeMinions.Clear();

        //// 3. 여우 비활성화
        //if (_foxController != null)
        //{
        //    _foxController.StopAllCoroutines();
        //    _foxController.gameObject.SetActive(false);
        //}
    }

    public List<GimmickItemObject> GetActiveItems()
    {
        // null이 된 항목(파괴됨) 정리 후 반환
        _activeItems.RemoveAll(item => item == null);
        return _activeItems;
    }

    public void RemoveItemFromList(GimmickItemObject item)
    {
        if (_activeItems.Contains(item))
        {
            _activeItems.Remove(item);
        }
    }
    #endregion

    // 데이터 전달용 내부 구조체
    private struct ItemSetupData
    {
        public Sprite Sprite;
        public bool IsTarget;
        public ItemSetupData(Sprite s, bool t) { Sprite = s; IsTarget = t; }
    }
}