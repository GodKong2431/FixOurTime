using UnityEngine;
using System.Collections;

// 여우의 광역 그림자 공격 장판 처리 클래스
public class FoxShadowAttack : MonoBehaviour
{
    #region Inspector Fields
    [Header("Components")]
    [Tooltip("공격 범위 표시 스프라이트 렌더러")]
    [SerializeField] private SpriteRenderer _indicatorRenderer;

    [Tooltip("피격 판정 콜라이더 (IsTrigger 체크)")]
    [SerializeField] private BoxCollider2D _hitBox;
    #endregion

    #region Private Fields
    private float _damage;
    private float _knockback;
    private float _warningTime;

    private bool _isHitting = false;
    #endregion

    #region Public Methods
    public void Initialize(float damage, float knockback, float warningTime)
    {
        _damage = damage;
        _knockback = knockback;
        _warningTime = warningTime;

        StartCoroutine(AttackProcess());
    }
    #endregion

    #region Main Logic
    private IEnumerator AttackProcess()
    {
        // 1. 경고 단계 (반투명)
        PrepareAttack();
        yield return new WaitForSeconds(_warningTime);

        // 2. 타격 단계 (불투명 + 판정 활성화)
        ExecuteAttack();
        yield return new WaitForSeconds(0.2f); // 타격 지속 시간

        // 3. 종료
        EndAttack();
    }

    private void PrepareAttack()
    {
        if (_hitBox != null) _hitBox.enabled = false;

        // 반투명 경고 표시
        if (_indicatorRenderer != null)
        {
            Color color = _indicatorRenderer.color;
            color.a = 0.3f;
            _indicatorRenderer.color = color;
        }
    }

    private void ExecuteAttack()
    {
        // 불투명하게 변경 및 히트박스 활성화
        if (_indicatorRenderer != null)
        {
            Color color = _indicatorRenderer.color;
            color.a = 1.0f;
            _indicatorRenderer.color = color;
        }

        if (_hitBox != null) _hitBox.enabled = true;
        _isHitting = true;
    }

    private void EndAttack()
    {
        if (_hitBox != null) _hitBox.enabled = false;
        _isHitting = false;
        Destroy(gameObject);
    }
    #endregion

    #region Collision Handling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 타격 중일 때만 데미지 적용
        if (_isHitting && collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(_damage, _knockback, transform.position);
            }
        }
    }
    #endregion
}