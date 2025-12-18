using System.Collections;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Ghost : MonoBehaviour
{
    [Header("속도")]
    [SerializeField] private float _speed;

    [Header("스폰 딜레이")]
    [SerializeField] private float _delay;

    [Header("스폰 타일")]
    [SerializeField] private GameObject[] _platforms;

    [Header("스폰 오프셋")]
    [SerializeField] private float _offsetY;

    [Header("유령 오브젝트")]
    [SerializeField] GameObject _GhostObj;

    private Vector2 _spawnPoint;

    private Collider2D spawnPlatformCol;

    private WaitForSeconds _spawnDelay;

    private Coroutine _spawnCoroutine;


    private Vector2 GetDispawnPoint() => new Vector2(_spawnPoint.x == spawnPlatformCol.bounds.min.x ? spawnPlatformCol.bounds.max.x: spawnPlatformCol.bounds.min.x, _spawnPoint.y);

    private void Awake()
    {
        _spawnDelay = new WaitForSeconds(_delay);
    }

    private void Start()
    {
        StartSpawn();
    }

    private void StartSpawn()
    {
        Debug.Log("시작");
        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    private void StopSpawn()
    {
        StopCoroutine(_spawnCoroutine);
    }

    private Vector2 GetSpawnPoint()
    {
        _spawnPoint = Vector2.zero;
        GameObject spawnPlatform = _platforms[Random.Range(0, _platforms.Length)];
        spawnPlatformCol = spawnPlatform.GetComponent<Collider2D>();
        _spawnPoint.x = Random.Range(0, 2) == 0 ? spawnPlatformCol.bounds.min.x : spawnPlatformCol.bounds.max.x;
        _spawnPoint.y = spawnPlatformCol.bounds.max.y + _offsetY;
        return _spawnPoint;
    }

    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            _GhostObj.transform.position = GetSpawnPoint();
            Vector2 dispawnPoint = GetDispawnPoint();

            Debug.DrawLine(_spawnPoint, dispawnPoint);

            _GhostObj.SetActive(true);
            while (Vector2.Distance(_GhostObj.transform.position, dispawnPoint) >= 0.01f)
            {
                _GhostObj.transform.position = Vector2.MoveTowards(_GhostObj.transform.position, dispawnPoint, _speed * Time.deltaTime);
                yield return null;
            }

            _GhostObj.SetActive(false);
            yield return _spawnDelay;
        }
    }
}
