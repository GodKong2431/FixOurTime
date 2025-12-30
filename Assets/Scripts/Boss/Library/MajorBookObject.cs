using UnityEngine;
using System.Collections;

// 기믹 실패 시 발사되는 전공책 투사체 클래스
public class MajorBookObject : MonoBehaviour
{
    #region Private Fields
    private Stage2Boss _boss;
    private Transform _target;
    private Vector3 _direction;
    private bool _isMoving = false;

    // 접촉 데미지 쿨타임 관리
    private float _contactDamageTimer = 0f;
    private const float CONTACT_DAMAGE_INTERVAL = 0.5f;

    private Animator _animator;
    private int _terrainMask;
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // 지형 감지용 레이어 마스크 설정
        _terrainMask = LayerMask.GetMask("Wall", "AbsolutelyGround");
        if (_terrainMask == 0) _terrainMask = LayerMask.GetMask("Default");
    }

    #region Public Methods
    public void Initialize(Stage2Boss boss, Transform target)
    {
        _boss = boss;
        _target = target;
        StartCoroutine(AttackRoutine());
        StartCoroutine(DotDamageRoutine());
    }
    #endregion

    #region Main Logic
    private IEnumerator AttackRoutine()
    {
        // 1차 돌진 준비
        yield return StartCoroutine(AimRoutine());
        yield return new WaitForSeconds(_boss.Data.BookFireDelay);

        // 1차 돌진 실행 (벽에 닿을 때까지)
        yield return StartCoroutine(MoveUntilCollision());

        // 벽에 박힌 후 잠시 대기
        yield return new WaitForSeconds(0.5f);

        // 2차 돌진 준비 (애니메이션 재개 및 재조준)
        if (_animator != null) _animator.speed = 1f;
        yield return StartCoroutine(AimRoutine());
        yield return new WaitForSeconds(0.2f);

        // 2차 돌진 실행
        yield return StartCoroutine(MoveUntilCollision());

        // 최종 소멸 처리
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator MoveUntilCollision()
    {
        _isMoving = true;
        float speed = _boss.Data.BookMoveSpeed;

        while (_isMoving)
        {
            float moveDistance = speed * Time.deltaTime;

            // 이동 경로상의 장애물 미리 감지
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, moveDistance, _terrainMask);

            if (hit.collider != null)
            {
                // 벽에 닿으면 멈추되, 살짝 뒤로 물러나서 다음 레이캐스트가 바로 닿지 않게 함
                transform.position = hit.point + (hit.normal * 0.1f);
                _isMoving = false;

                if (_animator != null) _animator.speed = 0f;
                yield break;
            }

            transform.position += _direction * moveDistance;
            yield return null;
        }
    }

    private IEnumerator AimRoutine()
    {
        float aimTimer = 0;
        float duration = _boss.Data.BookAimTime;

        while (aimTimer < duration)
        {
            aimTimer += Time.deltaTime;
            if (_target != null) UpdateRotationToTarget();
            yield return null;
        }
    }

    private void UpdateRotationToTarget()
    {
        _direction = (_target.position - transform.position).normalized;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // 책 주변 도트 데미지 (범위 내 접근 시)
    private IEnumerator DotDamageRoutine()
    {
        float dotDamage = _boss.Data.BookDotDamage;
        float dotRange = 3.0f;
        float dotInterval = 1.0f;
        float dotTimer = 0f;

        while (true)
        {
            if (_target != null)
            {
                float dist = Vector3.Distance(transform.position, _target.position);
                if (dist <= dotRange)
                {
                    dotTimer += Time.deltaTime;
                    if (dotTimer >= dotInterval)
                    {
                        dotTimer = 0f;
                        if (_target.TryGetComponent(out IDamageable target))
                        {
                            target.TakeDamage(dotDamage, 0f, transform.position);
                        }
                    }
                }
                else dotTimer = 0f;
            }
            yield return null;
        }
    }

    private IEnumerator DestroyRoutine()
    {
        _isMoving = false;
        if (_animator != null) _animator.speed = 0f;

        yield return new WaitForSeconds(_boss.Data.BookDurationAfterHit);
        Destroy(gameObject);
    }
    #endregion

    #region Collision Handling
    // 플레이어 충돌 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 움직이는 중일 때만 즉사급 데미지
        if (!_isMoving) return;

        if (collision.CompareTag("Player") && collision.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(_boss.Data.BookDirectDamage, _boss.Data.BookKnockback, transform.position);
            Destroy(gameObject);
        }
    }

    // 멈춰있을 때 접촉 데미지 처리
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 움직이는 중이 아닐 때(=벽에 박혀있을 때) 플레이어가 닿으면 지속 데미지
        if (_isMoving) return;

        if (collision.CompareTag("Player"))
        {
            _contactDamageTimer += Time.deltaTime;
            if (_contactDamageTimer >= CONTACT_DAMAGE_INTERVAL)
            {
                if (collision.TryGetComponent(out IDamageable target))
                {
                    float damage = _boss.Data.BookDirectDamage; // 박혀있을 때도 아프게 설정
                    target.TakeDamage(damage, 0f, transform.position);
                }
                _contactDamageTimer = 0f;
            }
        }
    }
    #endregion
}