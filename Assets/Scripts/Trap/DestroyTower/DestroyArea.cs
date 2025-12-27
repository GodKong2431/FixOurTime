using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyArea : MonoBehaviour
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

    private HashSet<Vector3Int> collapsedCells = new HashSet<Vector3Int>();
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Tilemap")) return;

        Bounds bounds = col.bounds;
        Vector3Int min = _tilemap.WorldToCell(bounds.min);
        Vector3Int max = _tilemap.WorldToCell(bounds.max);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (!_tilemap.HasTile(cell)) continue;
                if (collapsedCells.Contains(cell)) continue;

                Collapse(cell);
            }
        }
    }

    void Collapse(Vector3Int cell)
    {
        collapsedCells.Add(cell);

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
        GameObject p = Instantiate(_piecePrefab);
        p.transform.position = pos;
        p.transform.localScale = Vector3.one * _pieceSize;

        SpriteRenderer sr = p.GetComponent<SpriteRenderer>();
        sr.color = color;
        sr.sortingOrder = 10;

        Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;

        Vector2 dir = new Vector2(
            Random.Range(-0.5f, 0.5f),
            Random.Range(1f, 2f)
        );

        rb.AddForce(dir * _force, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-3f, 3f));

        Destroy(p, _lifeTime);
    }
}