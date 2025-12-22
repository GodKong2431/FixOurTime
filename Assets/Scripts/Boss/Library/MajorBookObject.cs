using UnityEngine;
using System.Collections;

// 기믹 실패 시 발사되는 전공책 투사체 클래스
public class MajorBookObject : MonoBehaviour
{
    #region Private Fields
    private Stage2Boss _boss;
    private Transform _target;
    private Vector3 _direction;
    #endregion

    #region Public Methods
    public void Initialize(Stage2Boss boss, Transform target)
    {
        _boss = boss;
        _target = target;
        StartCoroutine(AttackRoutine());
    }
    #endregion

    #region Main Logic
    private IEnumerator AttackRoutine()
    {
        // 1. 조준 단계
        yield return StartCoroutine(AimRoutine());

        // 2. 발사 준비 (방향 고정 및 대기)
        Debug.Log("전공책 조준 고정");

        yield return new WaitForSeconds(_boss.Data.BookFireDelay);

        // 3. 발사 (직진)
        yield return StartCoroutine(FireRoutine());
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

    private IEnumerator FireRoutine()
    {
        float speed = _boss.Data.BookMoveSpeed;

        // 도트 데미지 관련 변수
        float dotDamage = _boss.Data.BookDotDamage;
        float dotRange = 3.0f; // 책 주변 3m
        float dotInterval = 1.0f; // 1초 주기
        float dotTimer = 0f;

        while (true)
        {
            // 방향대로 직진
            transform.position += _direction * speed * Time.deltaTime;

            // 주변 플레이어 체크 및 도트 데미지
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
                            // 도트 데미지는 넉백 없음
                            target.TakeDamage(dotDamage, 0f, transform.position);
                        }
                    }
                }
                else
                {
                    dotTimer = 0f; // 범위 밖으로 나가면 타이머 초기화
                }
            }

            yield return null;
        }
    }
    #endregion

    #region Collision Handling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 벽 충돌 시 소멸
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        // 플레이어 직격
        else if (collision.CompareTag("Player") && collision.TryGetComponent(out IDamageable target))
        {
            // BossData 값 사용
            target.TakeDamage(_boss.Data.BookDirectDamage, _boss.Data.BookKnockback, transform.position);
            Destroy(gameObject);
        }
    }
    #endregion
}