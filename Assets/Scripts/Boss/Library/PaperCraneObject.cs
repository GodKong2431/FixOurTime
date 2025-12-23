using UnityEngine;
using System.Collections;

// 플레이어를 추적하고 속박하는 종이학 몬스터 클래스
public class PaperCraneObject : MonoBehaviour, IDamageable
{
    #region Inspector Fields
    [Header("Settings")]
    [Tooltip("높이 보정값")]
    [SerializeField] private float _addHeight = 0.5f;
    [Tooltip("전방 거리 보정값")]
    [SerializeField] private float _frontOffset = 1.0f;
    #endregion

    #region Private Fields
    private Stage2Boss _boss;
    private Transform _target;

    // 타겟 컴포넌트 캐싱
    private Collider2D _targetCollider;
    private SpriteRenderer _targetRenderer; // 방향 판별용 렌더러

    // 이동 속도
    private float _speed;
    private bool _isChasing = true;
    #endregion

    #region Public Methods
    public void Initialize(Stage2Boss boss, Transform target)
    {
        _boss = boss;
        _target = target;

        if (_target != null)
        {
            _targetCollider = _target.GetComponent<Collider2D>();
            _targetRenderer = _target.GetComponent<SpriteRenderer>();
        }

        // 플레이어 속도의 1.2배
        _speed = boss.Data.BasePlayerSpeed * boss.Data.CraneSpeedMultiplier;
    }

    // 피격 시 소멸
    public void TakeDamage(float damage, float knockback, Vector3 hitPos)
    {
        Debug.Log("종이학 파괴");
        Destroy(gameObject);
    }
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        if (_isChasing && _target != null)
        {
            MoveTowardsTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isChasing) return;

        // 플레이어와 충돌하면 속박
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(BindPlayerRoutine(collision.gameObject));
        }
    }
    #endregion

    #region Internal Logic
    private void MoveTowardsTarget()
    {
        // 타겟의 전방(바라보는 방향) 위치 계산
        Vector3 targetPos = GetTargetFrontPosition();

        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
    }

    // 플레이어가 바라보는 앞쪽 위치 반환
    private Vector3 GetTargetFrontPosition()
    {
        if (_target == null) return transform.position;

        // 1. 중앙 좌표 계산
        Vector3 centerPos = _target.position;
        if (_targetCollider != null) centerPos = _targetCollider.bounds.center;

        // 2. 바라보는 방향 계산 (Scale과 FlipX 둘 다 체크)
        float facingDir = 1f;

        // Case 1: Scale이 음수면 왼쪽
        if (_target.localScale.x < 0)
        {
            facingDir = -1f;
        }
        // Case 2: SpriteRenderer가 있고 flipX가 켜져 있으면 왼쪽
        else if (_targetRenderer != null && _targetRenderer.flipX)
        {
            facingDir = -1f;
        }

        // 중앙에서 보는 방향으로 조금 떨어진 위치
        return centerPos + new Vector3(facingDir * _frontOffset, _addHeight, 0);
    }

    private IEnumerator BindPlayerRoutine(GameObject playerObj)
    {
        _isChasing = false;
        Debug.Log("종이학: 플레이어 행동 불가");

        // 속박 적용
        if (playerObj.TryGetComponent(out IBindable bindTarget))
        {
            bindTarget.SetBind(_boss.Data.BindDuration);
        }

        float elapsed = 0f;
        float duration = _boss.Data.BindDuration;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (_target != null)
            {
                // 속박 중에도 플레이어 앞쪽에 계속 위치 (같이 이동/낙하)
                transform.position = GetTargetFrontPosition();
            }
            else
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }

        // 속박 실패 시 데미지
        if (playerObj != null && playerObj.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(_boss.Data.BindFailDamage, 0, transform.position);
        }

        Destroy(gameObject);
    }
    #endregion
}