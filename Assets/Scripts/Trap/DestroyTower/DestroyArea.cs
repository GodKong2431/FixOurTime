using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyArea : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed = 0.5f;

    [Header("대상 타일맵")]
    [SerializeField] private Tilemap _tilemap;

    private Collider2D _zoneCollider;

    private void Awake()
    {
        _zoneCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

        DestroyTilesInZone();
    }

    private void DestroyTilesInZone()
    {
        Bounds bounds = _zoneCollider.bounds;

        Vector3Int minCell = _tilemap.WorldToCell(bounds.min);
        Vector3Int maxCell = _tilemap.WorldToCell(bounds.max);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (!_tilemap.HasTile(cellPos))
                    continue;

                // 타일 제거
                _tilemap.SetTile(cellPos, null);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<TilemapCollider2D>())
            return;

        if(other.GetComponent<ItemObject>())
            _moveSpeed *= 2;

        if (other.TryGetComponent(out Player player))
        {
            player.TakeDamage(9999, 0, transform.position);
            return;
        }
            

        other.gameObject.SetActive(false);
    }
}