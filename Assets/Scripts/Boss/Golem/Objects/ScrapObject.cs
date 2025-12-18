using UnityEngine;

public class ScrapObject : MonoBehaviour
{
    private Vector3 _dir;
    private float _speed;
    private float _damage;

    private bool _isFragment; // 파편 여부
    private int _bounceCount = 0; // 튕긴 횟수
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
        // 바닥 레이어 가져오기
        int groundLayer = LayerMask.NameToLayer("Ground");

        // 1. 플레이어 피격
        if (collision.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(_damage, 5.0f, transform.position);
            Destroy(gameObject);
        }
        // 2. 벽 충돌 (파편이 아닐 때만 분열)
        else if (!_isFragment && collision.CompareTag("Wall"))
        {
            Split();
            Destroy(gameObject);
        }
        // 3. 파편일 경우에는 벽(태그), 바닥(레이어) 충돌 시 튕김
        else if (_isFragment)
        {
            bool isWall = collision.CompareTag("Wall");
            bool isGround = collision.gameObject.layer == groundLayer;

            if (isWall|| isGround)
            {
                _bounceCount++;
                // 최대 3회 튕김 후 소멸
                if (_bounceCount >= 2)
                {
                    Destroy(gameObject); // 2회 튕긴 후 소멸
                }
                else
                {
                    if(isWall)
                    {
                        // 벽에 닿으면 x축 반전
                        _dir.x *= -1; 
                    }
                    else if(isGround)
                    {
                        // 바닥에 닿으면 y축 반전
                        _dir.y *= -1;
                    }
                }
            }
        }
    }

    private void Split()
    {
        // 4방향 파편 생성 (대각선)
        Vector3[] dirs = 
            { new Vector3(-1, 1), new Vector3(1, 1), new Vector3(-1, -1), new Vector3(1, -1) };

        foreach (var d in dirs)
        {
            GameObject frag = Instantiate(gameObject, transform.position, Quaternion.identity);
            frag.transform.localScale = transform.localScale * 0.5f; // 크기 절반
            // 무한 루프 방지를 위해 isFragment = true 설정
            frag.GetComponent<ScrapObject>().Initialize(d.normalized, new BossData { scrapSpeed = _speed, scrapFragDamage = 5 }, true);
        }
    }
}