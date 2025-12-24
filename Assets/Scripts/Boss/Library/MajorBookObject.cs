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
    #endregion

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
        yield return StartCoroutine(AimRoutine());

        Debug.Log("전공책 조준 고정");
        yield return new WaitForSeconds(_boss.Data.BookFireDelay);

        _isMoving = true;
        float speed = _boss.Data.BookMoveSpeed;

        while (_isMoving)
        {
            transform.position += _direction * speed * Time.deltaTime;
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

    private IEnumerator StopAndPersist()
    {
        _isMoving = false;
        yield return new WaitForSeconds(_boss.Data.BookDurationAfterHit);
        Destroy(gameObject);
    }
    #endregion

    #region Collision Handling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isMoving) return;

        if (collision.CompareTag("Wall") || collision.CompareTag("Ground"))
        {
            StartCoroutine(StopAndPersist());
        }
        else if (collision.CompareTag("Player") && collision.TryGetComponent(out IDamageable target))
        {
            // 이동 중 직격 데미지
            target.TakeDamage(_boss.Data.BookDirectDamage, _boss.Data.BookKnockback, transform.position);
            Destroy(gameObject);
        }
    }

    // 멈춰있을 때 접촉 데미지 처리
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isMoving) return;

        if (collision.CompareTag("Player"))
        {
            _contactDamageTimer += Time.deltaTime;
            if (_contactDamageTimer >= CONTACT_DAMAGE_INTERVAL)
            {
                if (collision.TryGetComponent(out IDamageable target))
                {
                    // 멈춰있어도 이동 충돌과 동일한 데미지 적용
                    float damage = _boss.Data.BookDirectDamage;
                    target.TakeDamage(damage, 0f, transform.position);
                }
                _contactDamageTimer = 0f;
            }
        }
    }
    #endregion
}