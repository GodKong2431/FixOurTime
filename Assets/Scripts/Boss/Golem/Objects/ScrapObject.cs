using UnityEngine;

public class ScrapObject : MonoBehaviour
{
    private Vector3 _dir;
    private float _speed;
    private float _damage;
    private bool _isFragment; // 파편 여부

    public void Initialize(Vector3 dir, BossData data, bool isFragment = false)
    {
        _dir = dir;
        _speed = data.scrapSpeed;
        _damage = isFragment ? data.scrapFragDamage : data.scrapDamage;
        _isFragment = isFragment;

        Destroy(gameObject, 5f); // 안전장치
    }

    private void Update()
    {
        transform.Translate(_dir * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 플레이어 피격
        if (collision.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(_damage, 1f, transform.position);
            Destroy(gameObject);
        }
        // 2. 벽 충돌 (파편이 아닐 때만 분열)
        else if (collision.CompareTag("Wall") && !_isFragment)
        {
            Split();
            Destroy(gameObject);
        }
    }

    private void Split()
    {
        // 4방향 파편 생성 (대각선)
        Vector3[] dirs = { new Vector3(-1, 1), new Vector3(1, 1), new Vector3(-1, -1), new Vector3(1, -1) };

        foreach (var d in dirs)
        {
            GameObject frag = Instantiate(gameObject, transform.position, Quaternion.identity);
            frag.transform.localScale = transform.localScale * 0.5f; // 크기 절반
            // 무한 루프 방지를 위해 isFragment = true 설정
            frag.GetComponent<ScrapObject>().Initialize(d.normalized, new BossData { scrapSpeed = _speed, scrapFragDamage = 5 }, true);
        }
    }
}