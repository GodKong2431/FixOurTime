using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public enum GhostSpawnSide
    {
        Left,
        Right
    }

    [Header("속도")]
    [SerializeField] private float _speed = 3f;

    [Header("스폰 딜레이")]
    [SerializeField] private float _delay = 2f;

    [Header("스폰 타일")]
    [SerializeField] private GameObject[] _platforms;

    [Header("스폰 오프셋")]
    [SerializeField] private float _offsetY = 0.5f;

    [Header("유령 오브젝트")]
    [SerializeField] private GameObject _ghostObj;

    [Header("카메라 기준 스폰 방향")]
    [SerializeField] private GhostSpawnSide _spawnSide = GhostSpawnSide.Right;

    private Vector2 _spawnPoint;
    private Collider2D _spawnPlatformCol;
    private WaitForSeconds _spawnDelay;
    private Coroutine _spawnCoroutine;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spawnDelay = new WaitForSeconds(_delay);
        _spriteRenderer = _ghostObj.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartSpawn();
    }

    public void SetSpawnSide(GhostSpawnSide side)
    {
        _spawnSide = side;
    }

    public void StartSpawn()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    public void StopSpawn()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
    }

    private bool IsPlatformVisible(Collider2D col)
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 min = cam.WorldToViewportPoint(col.bounds.min);
        Vector3 max = cam.WorldToViewportPoint(col.bounds.max);

        return max.x > 0 && min.x < 1 && max.y > 0 && min.y < 1;
    }

    private bool IsCorrectSide(Collider2D col)
    {
        float camX = Camera.main.transform.position.x;
        float platformX = col.bounds.center.x;

        return _spawnSide == GhostSpawnSide.Right ? platformX > camX : platformX < camX;
    }

    private Collider2D GetValidPlatform()
    {
        List<Collider2D> candidates = new List<Collider2D>();

        foreach (GameObject platform in _platforms)
        {
            if (platform == null) continue;

            Collider2D col = platform.GetComponent<Collider2D>();
            if (col == null) continue;

            if (!IsPlatformVisible(col)) continue;
            if (!IsCorrectSide(col)) continue;

            candidates.Add(col);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private Vector2 GetSpawnPoint()
    {
        _spawnPlatformCol = GetValidPlatform();
        if (_spawnPlatformCol == null)
            return Vector2.zero;

        _spawnPoint.x = Random.Range(0, 2) == 0 ? _spawnPlatformCol.bounds.min.x : _spawnPlatformCol.bounds.max.x;

        _spawnPoint.y = _spawnPlatformCol.bounds.max.y + _offsetY;

        return _spawnPoint;
    }

    private Vector2 GetDispawnPoint()
    {
        return new Vector2(
            _spawnPoint.x == _spawnPlatformCol.bounds.min.x ? _spawnPlatformCol.bounds.max.x : _spawnPlatformCol.bounds.min.x,
            _spawnPoint.y
        );
    }

    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            Vector2 spawnPos = GetSpawnPoint();

            if (_spawnPlatformCol == null)
            {
                yield return _spawnDelay;
                continue;
            }

            _ghostObj.transform.position = spawnPos;
            Vector2 dispawnPoint = GetDispawnPoint();
            _spriteRenderer.flipX = spawnPos.x - dispawnPoint.x > 0;
            _ghostObj.SetActive(true);

            while (Vector2.Distance(_ghostObj.transform.position, dispawnPoint) > 0.01f)
            {
                _ghostObj.transform.position = Vector2.MoveTowards( _ghostObj.transform.position, dispawnPoint, _speed * Time.deltaTime);
                yield return null;
            }

            _ghostObj.SetActive(false);
            yield return _spawnDelay;
        }
    }
}