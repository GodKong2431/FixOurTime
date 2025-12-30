using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Pool;

public class DestsroyArea : MonoBehaviour
{
    [Header("타일맵")]
    [SerializeField] private Tilemap _tilemap;

    [Header("조각 프리펩")]
    [SerializeField] private GameObject[] _piecePrefab;

    [Header("조각 개수")]
    [SerializeField] private int _piecesCount = 6;

    [Header("조각 사이즈")]
    [SerializeField] private float _pieceSize = 0.4f;

    [Header("힘")]
    [SerializeField] private float _force = 2.5f;

    [Header("생존 시간")]
    [SerializeField] private float _lifeTime = 2f;

    [Header("풀 설정")]
    [SerializeField] private int _defaultPoolSize = 50;
    [SerializeField] private int _maxPoolSize = 200;

    [Header("속도")]
    [SerializeField] private float _moveSpeed = 0.5f;
    [SerializeField] private int _mulSpeed = 20;

    [Header("오프셋")]
    [SerializeField] private float offset = 20;

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
        GameObject obj = Instantiate(_piecePrefab[Random.Range(0, _piecePrefab.Length)]);
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
        transform.position += Vector3.up * _moveSpeed * Time.fixedDeltaTime;
        SoundManager.Instance.PlaySFX("SFX_Stage4_Earthquake");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent(out Player player))
        {
            SceneChanger.Instance.ChangeScene("stage4", true);
            return;
        }

        if(collision.TryGetComponent(out ItemObject item))
        {
            _moveSpeed *= _mulSpeed;
            collision.gameObject.SetActive(false);
        }
    }

    public void ResetPosition()
    {
        if (GameManager.Instance == null || GameManager.Instance.Player == null)
            return;

        float playerY = GameManager.Instance.Player.transform.position.y;

        // 플레이어 발밑 offset 위치로 즉시 이동
        transform.position = new Vector3(transform.position.x, playerY - offset, 0);

        // 아이템으로 빨라졌던 속도도 초기값으로 리셋
        _moveSpeed = 0.5f;

        Debug.Log("인스펙터 이벤트를 통해 DestroyArea 위치가 리셋되었습니다.");
    }
}