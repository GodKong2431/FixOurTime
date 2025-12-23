using UnityEngine;
using System.Collections;

// 기믹 실패 시 발사되는 전공책 투사체 클래스
public class MajorBookObject : MonoBehaviour
{
    #region Private Fields
    private Stage2Boss _boss;
    private Transform _target;
    private Vector3 _direction;
    private bool _isMoving = false; // 이동 중 여부
    #endregion

    #region Public Methods
    public void Initialize(Stage2Boss boss, Transform target)
    {
        _boss = boss;
        _target = target;
        StartCoroutine(AttackRoutine());

        // 생성 직후부터 도트 데미지 장판 가동
        StartCoroutine(DotDamageRoutine());
    }
    #endregion

    #region Main Logic
    private IEnumerator AttackRoutine()
    {
        // 1. 조준 단계
        yield return StartCoroutine(AimRoutine());

        // 2. 발사 준비
        Debug.Log("전공책 조준 고정");
        yield return new WaitForSeconds(_boss.Data.BookFireDelay);

        // 3. 발사
        _isMoving = true;
        float speed = _boss.Data.BookMoveSpeed;

        while (_isMoving)
        {
            // 설정된 방향으로 직진
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
            if (_target != null)
            {
                UpdateRotationToTarget();
            }
            yield return null;
        }
    }

    private void UpdateRotationToTarget()
    {
        _direction = (_target.position - transform.position).normalized;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // 책 주변 도트 데미지 처리
    private IEnumerator DotDamageRoutine()
    {
        float dotDamage = _boss.Data.BookDotDamage;
        float dotRange = 3.0f; // 범위 3m
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
                else
                {
                    dotTimer = 0f;
                }
            }
            yield return null;
        }
    }

    // 충돌 후 유지하다가 소멸
    private IEnumerator StopAndPersist()
    {
        _isMoving = false; // 이동 정지

        // 충돌 지점에서 대기 (장판 역할)
        yield return new WaitForSeconds(_boss.Data.BookDurationAfterHit);

        Destroy(gameObject);
    }
    #endregion

    #region Collision Handling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isMoving) return;

        // 벽이나 바닥 충돌 시 정지
        if (collision.CompareTag("Wall") || collision.CompareTag("Ground"))
        {
            StartCoroutine(StopAndPersist());
        }
        // 플레이어 직격 시 데미지 및 소멸
        else if (collision.CompareTag("Player") && collision.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(_boss.Data.BookDirectDamage, _boss.Data.BookKnockback, transform.position);
            Destroy(gameObject);
        }
    }
    #endregion
}