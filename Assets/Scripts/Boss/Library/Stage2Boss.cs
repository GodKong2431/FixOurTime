using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Sub-Systems")]
    [Tooltip("보스전 소환 여우(Fox) 컨트롤러")]
    [SerializeField] private FoxController _foxController;

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

    #region Public Properties
    public Transform[] SpawnPoints => _itemSpawnPoints;
    public Boss2Data Data => _bossData;
    public Transform CenterPoint => transform;
    #endregion

    #region Private Fields
    // 로직 제어 변수
    private bool _isActivated = false;
    private bool _isGimmickRunning = false;

    // 생성된 오브젝트 관리용 리스트
    private List<GimmickItemObject> _activeItems = new List<GimmickItemObject>();
    private List<GameObject> _activeMinions = new List<GameObject>();

    // 기믹 카운트
    private int _collectedCount = 0;
    private const int TARGET_COUNT = 4;
    #endregion

    #region Unity Lifecycle
    public override void ActivateBoss()
    {
        if (_isActivated) return;
        _isActivated = true;

        ClearStageObjects();
        StartCoroutine(CraneSpawnRoutine());
        StartCoroutine(MainGimmickRoutine());
    }

    public override void ResetBoss()
    {
        base.ResetBoss();

        StopAllCoroutines();
        CancelInvoke(nameof(SpawnFox));

        ClearStageObjects();

        _isActivated = false;
        _isGimmickRunning = false;

        Debug.Log("Stage 2 리셋 완료");
    }

    protected override void Die()
    {
        StopAllCoroutines();
        CancelInvoke(nameof(SpawnFox));
        ClearStageObjects();

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
            yield return new WaitUntil(() => _isGimmickRunning == false);
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
        _activeMinions.Add(craneObj);
        if (craneObj.TryGetComponent(out PaperCraneObject craneScript))
        {
            craneScript.Initialize(this, _playerTarget);
        }
    }

    private void SpawnFox()
    {
        if (!_isGimmickRunning) return;
        if (_foxController != null)
        {
            _foxController.ActivateFox(this, _playerTarget);
        }
    }

    private void StartGimmickRound()
    {
        _isGimmickRunning = true;
        _collectedCount = 0;
        ClearAllItems(false);

        // 1. 전체 스프라이트 목록 복사
        List<Sprite> spritePool = new List<Sprite>(_itemSprites);

        // 2. 정답을 선정하기 전에 리스트를 무작위로 섞음 (이 코드가 없어서 매번 똑같은게 나왔음)
        ShuffleList(spritePool);

        // 3. 섞인 리스트의 앞쪽 4개를 정답으로 사용
        List<Sprite> targetSprites = SelectTargetSprites(spritePool);

        // 4. 그 뒤의 리스트를 함정으로 사용 (정답과 겹치지 않게 됨)
        List<Sprite> trapSprites = SelectTrapSprites(spritePool, targetSprites.Count);

        if (_uiManager != null) _uiManager.ShowTargetItems(targetSprites);

        // 5. 맵에 생성할 데이터 구성
        List<ItemSetupData> spawnList = new List<ItemSetupData>();
        foreach (var s in targetSprites) spawnList.Add(new ItemSetupData(s, true));
        foreach (var s in trapSprites) spawnList.Add(new ItemSetupData(s, false));

        // 6. 생성 위치도 랜덤하게 섞기
        ShuffleList(spawnList);
        PlaceGimmickItems(spawnList);

        CancelInvoke(nameof(SpawnFox));
        Invoke(nameof(SpawnFox), _bossData.FoxSpawnDelay);
    }

    private List<Sprite> SelectTargetSprites(List<Sprite> pool)
    {
        List<Sprite> targets = new List<Sprite>();
        for (int i = 0; i < TARGET_COUNT; i++)
        {
            if (i < pool.Count) targets.Add(pool[i]);
            else targets.Add(pool[0]);
        }
        return targets;
    }

    private List<Sprite> SelectTrapSprites(List<Sprite> pool, int offset)
    {
        List<Sprite> traps = new List<Sprite>();
        int trapCount = 6;
        for (int i = 0; i < trapCount; i++)
        {
            if (pool.Count >= offset + trapCount) traps.Add(pool[offset + i]);
            else traps.Add(_itemSprites[Random.Range(0, _itemSprites.Length)]);
        }
        return traps;
    }

    private void PlaceGimmickItems(List<ItemSetupData> list)
    {
        for (int i = 0; i < _itemSpawnPoints.Length && i < list.Count; i++)
        {
            GameObject o = Instantiate(_gimmickItemPrefab, _itemSpawnPoints[i].position, Quaternion.identity);
            if (o.TryGetComponent(out GimmickItemObject s))
            {
                s.Initialize(this, list[i].Sprite, list[i].IsTarget);
                _activeItems.Add(s);
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T t = list[i];
            int r = Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = t;
        }
    }
    #endregion

    #region Event Handlers (Public)
    public void OnPlayerCollectItem(bool isTarget, Sprite itemSprite = null)
    {
        if (isTarget)
        {
            _collectedCount++;
            if (_uiManager != null) _uiManager.MarkItemAsCollected(itemSprite);
            if (_collectedCount >= TARGET_COUNT) GimmickSuccess();
        }
        else
        {
            GimmickFail(keepFox: true);

            if (_foxController != null && _foxController.gameObject.activeSelf)
            {
                _foxController.ForceRetreat();
            }
        }
    }

    public void OnFoxEatItem(bool isTarget)
    {
        if (isTarget)
        {
            Debug.Log("여우가 정답 아이템을 먹었습니다.");
            GimmickFail(keepFox: true);
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

        // 성공 시에도 여우가 벽으로 도망가도록 keepFox=true 전달 후 강제 퇴장 명령
        ClearAllItems(true);
        if (_foxController != null && _foxController.gameObject.activeSelf)
        {
            _foxController.ForceRetreat();
        }

        _isGimmickRunning = false;
    }

    private void GimmickFail(bool keepFox)
    {
        ClearAllItems(keepFox);
        FireMajorBook();
        _isGimmickRunning = false;
    }

    private void FireMajorBook()
    {
        Vector3 spawnPos = transform.position + Vector3.up * 5;
        GameObject bookObj = Instantiate(_majorBookPrefab, spawnPos, Quaternion.identity);
        _activeMinions.Add(bookObj);
        if (bookObj.TryGetComponent(out MajorBookObject bookScript))
        {
            bookScript.Initialize(this, _playerTarget);
        }
    }

    private void ClearAllItems(bool keepFox)
    {
        CancelInvoke(nameof(SpawnFox));

        if (!keepFox && _foxController != null && _foxController.gameObject.activeSelf)
        {
            _foxController.StopAllCoroutines();
            _foxController.gameObject.SetActive(false);
        }

        foreach (var item in _activeItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        _activeItems.Clear();

        if (_uiManager != null) _uiManager.HideUI();
    }

    private void ClearStageObjects()
    {
        ClearAllItems(false);
        foreach (var minion in _activeMinions)
        {
            if (minion != null) Destroy(minion);
        }
        _activeMinions.Clear();
    }

    public List<GimmickItemObject> GetActiveItems()
    {
        _activeItems.RemoveAll(item => item == null);
        return _activeItems;
    }

    public void RemoveItemFromList(GimmickItemObject item)
    {
        if (_activeItems.Contains(item)) _activeItems.Remove(item);
    }
    #endregion

    private struct ItemSetupData
    {
        public Sprite Sprite;
        public bool IsTarget;
        public ItemSetupData(Sprite s, bool t) { Sprite = s; IsTarget = t; }
    }
}