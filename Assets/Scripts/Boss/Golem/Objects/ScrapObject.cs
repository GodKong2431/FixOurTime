using UnityEngine;

public class ScrapObject : MonoBehaviour
{
    private Vector3 _dir;
    private float _speed;
    private float _damage;
    private Boss1Data _data; // 데이터 저장


    [Header("파편 이미지 설정")]
    [SerializeField] private Sprite[] _fragmentSprites; // 나사, 너트, 볼트 이미지 배열

    private bool _isFragment; // 파편 여부
    private int _bounceCount = 0; // 튕긴 횟수
    public void Initialize(Vector3 dir, Boss1Data data, bool isFragment = false)
    {
        _data = data;
        _dir = dir;
        _speed = data.ScrapSpeed;
        _damage = isFragment ? data.ScrapFragDamage : data.ScrapDamage;
        _isFragment = isFragment;

        // 파편이라면 저장해둔 스프라이트 중 하나로 랜덤 변경
        if (_isFragment && _fragmentSprites != null && _fragmentSprites.Length > 0)
        {
            GetComponent<SpriteRenderer>().sprite = _fragmentSprites[Random.Range(0, _fragmentSprites.Length)];
        }

        Destroy(gameObject, data.ScrapLifeTime); // 안전장치
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
            float kb = _data != null ? _data.ScrapKnockback : 5f; // 넉백 받아오기
            target.TakeDamage(_damage, kb, transform.position);
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
        float offset = 1.0f; 
        Vector3 spawnPos = transform.position - (_dir.normalized * offset);

        // 반대 방향 x축 계산
        float reflectX = _dir.x > 0 ? -1f : 1f;

        // 부채꼴 4갈래
        Vector3[] dirs = new Vector3[]
        {
            new Vector3(reflectX, 0.5f, 0).normalized,
            new Vector3(reflectX, 1.5f, 0).normalized,
            new Vector3(reflectX, -0.5f, 0).normalized,
            new Vector3(reflectX, -1.5f, 0).normalized,
        };

        foreach(var d in dirs)
        {
            GameObject frag = Instantiate(gameObject, spawnPos, Quaternion.identity);

            frag.transform.localScale = transform.localScale * 1.0f;

            // 파편 생성
            frag.GetComponent<ScrapObject>().Initialize(d, _data, true);
        }
    }
}