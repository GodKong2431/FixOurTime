using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class PaperCraneSpawner : MonoBehaviour
{
    
    
    [Header("종이학 프리펩")]
    [SerializeField] private GameObject _paperCranePrefab;
    [Header("스폰 포인트")]
    [SerializeField] private GameObject[] _paperSpawnPoint;
    [Header("스폰 딜레이")]
    [SerializeField] private float _spawnDelay = 5f;

    ObjectPool<GameObject> _paperCranePool;

    List<GameObject> _activeCranes = new List<GameObject>();

    GameObject _player;

    WaitForSeconds _delay;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _delay = new WaitForSeconds(_spawnDelay);
        _paperCranePool = new ObjectPool<GameObject>
            (
                createFunc:() =>
                {
                    GameObject obj = Instantiate(_paperCranePrefab);
                    obj.GetComponent<PaperCrane>().Init(_player, this);
                    return obj;
                },
                actionOnGet:(obj) =>
                {
                    obj.transform.position = _paperSpawnPoint[Random.Range(0, _paperSpawnPoint.Length)].transform.position;
                    obj.SetActive(true);

                    _activeCranes.Add(obj);
                },
                actionOnRelease:(obj) =>
                {
                    obj.SetActive(false);
                },
                actionOnDestroy:(obj) =>
                {
                    Destroy(obj);
                },
                defaultCapacity:10,
                maxSize: 100
            );
    }

    private void Start()
    {
        StartCoroutine(StartSpawn());
    }

    IEnumerator StartSpawn()
    {
        while(true)
        {
            _paperCranePool.Get();
            yield return _delay;
        }
    }

    public void StopAndClearSpawns()
    {
        StopAllCoroutines();

        // 리스트를 복사해서 순회 (순회 중 원본 리스트 수정 방지)
        var targets = new List<GameObject>(_activeCranes);

        foreach (var crane in targets)
        {
            if (crane.activeSelf)
            {
                // 강제로 풀에 반환
                _paperCranePool.Release(crane);
            }
        }

        // 관리 리스트 초기화
        _activeCranes.Clear();
    }
    public void Release(GameObject paperCrane)
    {
        if (_activeCranes.Contains(paperCrane))
        {
            _activeCranes.Remove(paperCrane);
        }

        _paperCranePool.Release(paperCrane);
    }
}
