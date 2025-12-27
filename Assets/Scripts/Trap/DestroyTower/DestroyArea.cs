using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider2D))]
public class TilePixelCollapse : MonoBehaviour
{
    [Header("타일맵")]
    public Tilemap _tilemap;

    [Header("조각 프리펩")]
    public GameObject _piecePrefab;

    [Header("조각 개수")]
    public int _piecesCount = 6;

    [Header("조각 사이즈")]
    public float _pieceSize = 0.4f;

    [Header("힘")]
    public float _force = 2.5f;

    [Header("생존 시간")]
    public float _lifeTime = 2f;

    [Header("풀 설정")]
    public int _defaultPoolSize = 50;
    public int _maxPoolSize = 200;

    private HashSet<Vector3Int> _collapsedCells = new HashSet<Vector3Int>();
    private Collider2D _col;

    private ObjectPool<GameObject> _piecePool;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;

        _piecePool = new ObjectPool<GameObject>(
            CreatePixel,
            OnGetPixel,
            OnReleasePixel,
            OnDestroyPixel,
            false,
            _defaultPoolSize,
            _maxPoolSize
        );
    }

    GameObject CreatePixel()
    {
        GameObject obj = Instantiate(_piecePrefab);
        obj.SetActive(false);
        return obj;
    }

    void OnGetPixel(GameObject obj)
    {
        obj.SetActive(true);
    }

    void OnReleasePixel(GameObject obj)
    {
        obj.SetActive(false);
    }

    void OnDestroyPixel(GameObject obj)
    {
        Destroy(obj);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Tilemap")) return;

        Bounds bounds = _col.bounds;
        Vector3Int min = _tilemap.WorldToCell(bounds.min);
        Vector3Int max = _tilemap.WorldToCell(bounds.max);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (!_tilemap.HasTile(cell)) continue;
                if (_collapsedCells.Contains(cell)) continue;

                Collapse(cell);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position += Vector3.up * 0.5f * Time.fixedDeltaTime;
    }

    void Collapse(Vector3Int cell)
    {
        _collapsedCells.Add(cell);

        Tile tile = _tilemap.GetTile(cell) as Tile;
        if (tile == null) return;

        Vector3 center = _tilemap.GetCellCenterWorld(cell);
        Color finalColor = SpriteAverageColorCache.GetAverageColor(tile.sprite);

        _tilemap.SetTile(cell, null);

        for (int i = 0; i < _piecesCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.3f;
            SpawnPixel(center + (Vector3)offset, finalColor);
        }
    }

    void SpawnPixel(Vector3 pos, Color color)
    {
        GameObject p = _piecePool.Get();

        p.transform.position = pos;
        p.transform.localScale = Vector3.one * _pieceSize;

        SpriteRenderer sr = p.GetComponent<SpriteRenderer>();
        sr.color = color;
        sr.sortingOrder = 10;

        Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Vector2 dir = new Vector2(
            Random.Range(-0.5f, 0.5f),
            Random.Range(1f, 2f)
        );

        rb.AddForce(dir * _force, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-3f, 3f));

        StartCoroutine(ReleaseAfterTime(p, _lifeTime));
    }

    IEnumerator ReleaseAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        _piecePool.Release(obj);
    }
}