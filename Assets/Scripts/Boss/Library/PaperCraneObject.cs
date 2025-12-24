using UnityEngine;
using System.Collections;

// 플레이어를 추적하고 속박하는 종이학 몬스터 클래스
public class PaperCraneObject : MonoBehaviour, IDamageable
{
    #region Inspector Fields
    [Header("Settings")]
    [Tooltip("높이 보정값")]
    [SerializeField] private float _addHeight = 0.7f;
    [Tooltip("전방 거리 보정값")]
    [SerializeField] private float _frontOffset = 0.5f;
    #endregion

    #region Private Fields
    private Stage2Boss _boss;
    private Transform _target;

    // 타겟 컴포넌트 캐싱
    private Collider2D _targetCollider;
    private SpriteRenderer _targetRenderer;

    // 이동 속도
    private float _speed;
    private bool _isChasing = true;

    // 물리 연산을 제어할 리지드바디
    private Rigidbody2D _rb;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // 리지드바디가 절대로 '수면(Sleep)' 상태로 가지 않도록 설정
        // 이 설정이 없으면 플레이어에게 붙어서 가만히 있을 때 물리 연산이 꺼져서 안 맞게 됨
        if (_rb != null)
        {
            _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
    }

    private void Update()
    {
        // 추적 중일 때는 단순 이동
        if (_isChasing && _target != null)
        {
            MoveTowardsTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isChasing) return;

        // 플레이어와 충돌하면 속박 시도
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(BindPlayerRoutine(collision.gameObject));
        }
    }
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

        _speed = boss.Data.BasePlayerSpeed * boss.Data.CraneSpeedMultiplier;
    }

    public void TakeDamage(float damage, float knockback, Vector3 hitPos)
    {
        Debug.Log("종이학 파괴");
        Destroy(gameObject);
    }
    #endregion

    #region Internal Logic
    private void MoveTowardsTarget()
    {
        Vector3 targetPos = GetTargetFrontPosition();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
    }

    private Vector3 GetTargetFrontPosition()
    {
        if (_target == null) return transform.position;

        Vector3 centerPos = _target.position;
        if (_targetCollider != null) centerPos = _targetCollider.bounds.center;

        float facingDir = 1f;
        if (_target.localScale.x < 0) facingDir = -1f;
        else if (_targetRenderer != null && _targetRenderer.flipX) facingDir = -1f;

        return centerPos + new Vector3(facingDir * _frontOffset, _addHeight, 0);
    }

    private IEnumerator BindPlayerRoutine(GameObject playerObj)
    {
        _isChasing = false;

        // Kinematic으로 전환하여 물리적인 밀림 방지
        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("종이학: 플레이어 행동 불가");

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
                // transform.position 대신 MovePosition 사용
                // 물리 엔진에게 순간이동이 아니라 이동 중임을 알려 충돌 판정을 유지함
                Vector3 destPos = GetTargetFrontPosition();

                if (_rb != null) _rb.MovePosition(destPos);
                else transform.position = destPos;
            }
            else
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }

        // 속박 실패(시간 초과) 시 데미지
        if (playerObj != null && playerObj.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(_boss.Data.BindFailDamage, 0, transform.position);
        }

        Destroy(gameObject);
    }
    #endregion
}