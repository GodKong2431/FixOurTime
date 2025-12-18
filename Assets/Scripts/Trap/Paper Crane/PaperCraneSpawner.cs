using System.Collections;
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

    public void Release(GameObject paperCrane)
    {
        _paperCranePool.Release(paperCrane);
    }
}
