using System.Collections;
using UnityEngine;

public class DevilHand : DamageableTrapBase
{
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed = 18f;

    [Header("공격 속도")]
    [SerializeField] private float _attackSpeed = 18f;

    private Transform _bossTransform; // 보스 위치 참조
    private Vector2 _startOffset;     // 보스 기준 시작 위치 (오프셋)
    private Vector2 _returnPos;  // 초기 로컬 위치 (복귀용)

    private Coroutine _moveCoroutine;
    private bool _hitGround;

    // 외부에서 손이 움직이는 중인지 확인하는 프로퍼티
    public bool IsBusy => _moveCoroutine != null;

    private void Awake()
    {
        if (transform.parent != null)
        {
            _bossTransform = transform.parent;
            _returnPos = transform.localPosition;
        }
    }

    public void Configure(float moveSpeed, float attackSpeed, int damage)
    {
        _moveSpeed = moveSpeed;
        _attackSpeed = attackSpeed;
        this._trapDamage = damage;
    }

    /// <summary>
    /// 패턴 시작: 부모 해제하고 시작 위치로 '이동' 시작 (순간이동 X)
    /// </summary>
    public void BeginPattern(Vector2 startOffset)
    {
        _startOffset = startOffset;
        _hitGround = false;

        StopCurrentCoroutine(); // 기존 동작 정지

        transform.SetParent(null); // 부모에서 분리

  
        // 부드럽게 이동하는 코루틴 시작
        MoveToStartPos();
    }

    /// <summary>
    /// 시작 위치(보스 + 오프셋)로 부드럽게 이동
    /// </summary>
    public void MoveToStartPos()
    {
        StopCurrentCoroutine();
        _moveCoroutine = StartCoroutine(MoveToStartPosCoroutine());
    }

    private IEnumerator MoveToStartPosCoroutine()
    {
        // 보스가 살아있는 동안 목표 위치로 계속 이동
        while (_bossTransform != null)
        {
            Vector2 targetPos = (Vector2)_bossTransform.position + _startOffset;

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPos,
                _moveSpeed * Time.deltaTime
            );

            // 도착 판정
            if (Vector2.Distance(transform.position, targetPos) < 0.05f)
            {
                transform.position = targetPos; // 오차 보정
                break;
            }
            yield return null;
        }
        _moveCoroutine = null; // 이동 끝
    }

    /// <summary>
    /// 공격 수행
    /// </summary>
    public void Attack(Vector2 direction)
    {
        _hitGround = false;
        StopCurrentCoroutine();
        _moveCoroutine = StartCoroutine(AttackCoroutine(direction.normalized));
    }

    private IEnumerator AttackCoroutine(Vector2 dir)
    {
        while (!_hitGround)
        {
            transform.position += (Vector3)(dir * _attackSpeed * Time.deltaTime);
            yield return null;
        }
        _moveCoroutine = null; // 공격 끝
    }

    /// <summary>
    /// 스파이럴(회전) 공격
    /// </summary>
    public void SpiralAttack(Vector2 center, float offset, float duration)
    {
        StopCurrentCoroutine();
        Vector2 start = transform.position;
        Vector2 end = center;
        _moveCoroutine = StartCoroutine(SpiralAttackCoroutine(start, end, offset, duration));
    }

    IEnumerator SpiralAttackCoroutine(Vector2 start, Vector2 end, float offset, float duration)
    {
        Vector2 center = (start + end) * 0.5f;

        float xDist = Mathf.Abs(start.x - center.x);
        float dir = Mathf.Sign(end.x - start.x);
        if (dir == 0) dir = 1f;

        Vector2 offsetPos = center + Vector2.right * dir * xDist * offset;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector2 a = Vector2.Lerp(start, offsetPos, t);
            Vector2 b = Vector2.Lerp(offsetPos, end, t);
            Vector2 pos = Vector2.Lerp(a, b, t);

            transform.position = pos;
            yield return null;
        }
        transform.position = end;
        _moveCoroutine = null; 
    }

    /// <summary>
    /// 복귀
    /// </summary>
    public void MoveToReturnPos()
    {
        _hitGround = false;
        StopCurrentCoroutine();
        _moveCoroutine = StartCoroutine(ReturnCoroutine());
    }

    private IEnumerator ReturnCoroutine()
    {
        while (_bossTransform != null)
        {
            // 로컬 좌표(_returnPos)를 월드 좌표로 변환하여 이동
            Vector2 targetWorldPos = _bossTransform.TransformPoint(_returnPos);

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetWorldPos,
                _moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, targetWorldPos) < 0.05f)
            {
                transform.position = targetWorldPos;
                break;
            }
            yield return null;
        }

        // 도착하면 다시 자식으로 붙임
        if (_bossTransform != null)
        {
            transform.SetParent(_bossTransform);
            transform.localPosition = _returnPos;
        }
        _moveCoroutine = null; 
    }

    private void StopCurrentCoroutine()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.CompareTag("BossGround"))
        {
            _hitGround = true;
        }
    }

    public void ForceReturn()
    {
        StopCurrentCoroutine();
        if (_bossTransform != null)
        {
            transform.SetParent(_bossTransform);
            transform.localPosition = _returnPos;
        }
        if (_bossTransform != null && !_bossTransform.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
}