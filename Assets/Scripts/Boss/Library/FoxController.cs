using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 보스 여우 패턴 및 AI 제어 클래스
public class FoxController : MonoBehaviour, IDamageable
{
    #region Inspector Fields
    [Header("Prefabs & References")]
    [Tooltip("여우가 그림자 공격 시 소환할 장판 프리팹")]
    [SerializeField] private GameObject _shadowAttackPrefab;
    [Tooltip("플레이어 발견/공격 시 머리 위에 뜰 이펙트 프리팹")]
    [SerializeField] private GameObject _detectEffectPrefab;

    [Tooltip("그림자 상태일 때 사용할 스프라이트")]
    [SerializeField] private Sprite _shadowSprite;
    [Tooltip("기본 여우 스프라이트")]
    [SerializeField] private Sprite _normalSprite;

    [Header("Check Settings")]
    [Tooltip("바닥 감지 레이어 (필수 설정: Ground)")]
    [SerializeField] private LayerMask _groundLayer;

    [Header("Movement Settings")]
    [Tooltip("점프 전 준비 시간 (초)")]
    [SerializeField] private float _jumpDelay = 0.4f;
    #endregion

    #region Private Fields
    private Stage2Boss _boss;
    private Transform _player;
    private bool _isActive = false;

    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private Rigidbody2D _rb;

    private enum FoxState
    {
        Idle,
        ChasingBook,
        PrepareJump,
        Jumping,
        Eating,
        Retreat
    }
    private FoxState _state = FoxState.Idle;

    private GimmickItemObject _targetItem;
    private bool _isShadowMode = false;
    private Color _originalColor;

    private float _groundCheckRadius = 0.4f;
    private float _gravityScale = 3f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        if (_renderer != null) _originalColor = _renderer.color;
        _rb.gravityScale = _gravityScale;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;

        if (_state == FoxState.Retreat)
        {
            HandleRetreat();
            UpdateVisuals();
            return;
        }

        if (_state == FoxState.PrepareJump)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }

        if (_state == FoxState.Jumping)
        {
            if (_rb.linearVelocity.y <= 0 && CheckGrounded())
            {
                _state = FoxState.ChasingBook;
            }
        }
        else if (_state == FoxState.ChasingBook && _targetItem != null)
        {
            MoveToTarget(_targetItem.transform.position, _boss.Data.FoxMoveSpeed);
        }

        UpdateVisuals();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_state == FoxState.Retreat)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                Debug.Log("여우 벽 도달, 즉시 소멸");
                gameObject.SetActive(false);
                _isActive = false;
            }
        }
    }
    #endregion

    #region Public Methods
    public void ActivateFox(Stage2Boss boss, Transform player)
    {
        _boss = boss;
        _player = player;
        _isActive = true;
        _isShadowMode = false;

        if (_collider == null) _collider = GetComponent<Collider2D>();

        TeleportToRandomBook();

        gameObject.SetActive(true);
        EnablePhysics(true);

        StartCoroutine(AI_RoutineLoop());
    }

    public void ForceRetreat()
    {
        if (!_isActive) return;
        Debug.Log("여우 강제 퇴장 명령 수신");

        StopAllCoroutines();
        _state = FoxState.Retreat;
        EnablePhysics(true);
    }

    public void TakeDamage(float damage, float knockback, Vector3 hitPos)
    {
        // 피격 로직 유지
    }
    #endregion

    #region AI Logic
    private IEnumerator AI_RoutineLoop()
    {
        SetTarget(FindRandomBook());

        while (_isActive)
        {
            if (_targetItem == null)
            {
                SetTarget(FindNearestBook());
                yield return null;
                continue;
            }

            _state = FoxState.ChasingBook;

            while (_targetItem != null)
            {
                float dist = Vector2.Distance(transform.position, _targetItem.transform.position);

                // 땅에 있고 & 속도가 안정적일 때만 도착 인정
                if (dist <= 1.0f && CheckGrounded() && Mathf.Abs(_rb.linearVelocity.y) < 0.1f)
                {
                    break;
                }
                yield return null;
            }

            if (_targetItem == null) continue;

            yield return StartCoroutine(EatBookRoutine());
        }
    }

    private IEnumerator EatBookRoutine()
    {
        _state = FoxState.Eating;
        _rb.linearVelocity = Vector2.zero;
        Debug.Log("여우 책 먹는 중 (3초)");

        float elapsed = 0f;
        while (elapsed < 3.0f)
        {
            elapsed += Time.deltaTime;

            if (_targetItem == null || Vector2.Distance(transform.position, _targetItem.transform.position) > 1.2f)
            {
                Debug.Log("여우 먹기 취소 (멀어짐)");
                _state = FoxState.ChasingBook;
                yield break;
            }
            yield return null;
        }

        if (_targetItem != null)
        {
            bool isCorrect = _targetItem.IsTarget;
            _boss.OnFoxEatItem(isCorrect);

            _boss.RemoveItemFromList(_targetItem);
            Destroy(_targetItem.gameObject);
            _targetItem = null;

            if (isCorrect)
            {
                Debug.Log("여우 정답 획득, 벽으로 퇴장");
                _state = FoxState.Retreat;
            }
            else
            {
                Debug.Log("여우 오답, 가장 가까운 책으로 이동");
                SetTarget(FindNearestBook());
                _state = FoxState.ChasingBook;
            }
        }
    }
    #endregion

    #region Movement & Physics
    private void MoveToTarget(Vector3 targetPos, float speed)
    {
        if (!CheckGrounded()) return;

        // 중간 발판 탐색
        if (targetPos.y - transform.position.y > 3.0f)
        {
            Vector3? intermediate = GetIntermediatePlatform(targetPos);
            if (intermediate.HasValue) targetPos = intermediate.Value;
        }

        float xDiff = targetPos.x - transform.position.x;
        float yDiff = targetPos.y - transform.position.y;
        float dirX = Mathf.Sign(xDiff);

        // 우회 로직: 목표가 아래에 있으면 수평 거리 상관없이 계속 길을 찾음
        bool isTargetBelow = (yDiff < -1.0f);

        if (isTargetBelow)
        {
            // 보는 방향으로 계속 전진
            float currentFacing = _renderer.flipX ? -1f : 1f;
            dirX = currentFacing;

            // 진짜 벽(Trigger 아님)을 만나면 뒤로 돎
            Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, 1.0f, _groundLayer);

            if (hit.collider != null)
            {
                if (!hit.collider.isTrigger && hit.collider.gameObject != _targetItem.gameObject)
                {
                    dirX *= -1f;
                }
            }
        }
        else
        {
            // 일반적인 경우: 가까우면 멈춤
            if (Mathf.Abs(xDiff) < 0.2f) dirX = 0;
        }

        if (dirX != 0) _renderer.flipX = (dirX < 0);

        bool isHighTarget = yDiff > 0.6f;
        bool isGapAhead = IsGapAhead(dirX);

        // 아래로 내려가는 중일 때는 낭떠러지를 만나도 점프하지 않고 그냥 떨어짐
        bool needJump = isHighTarget || (isGapAhead && !isTargetBelow);

        if (needJump)
        {
            if (isHighTarget && Mathf.Abs(xDiff) > 2.5f && !isGapAhead)
            {
                _rb.linearVelocity = new Vector2(dirX * speed, _rb.linearVelocity.y);
            }
            else
            {
                StartCoroutine(JumpRoutine(targetPos));
            }
        }
        else
        {
            // 평지 이동
            _rb.linearVelocity = new Vector2(dirX * speed, _rb.linearVelocity.y);
        }
    }

    private IEnumerator JumpRoutine(Vector3 targetPos)
    {
        _state = FoxState.PrepareJump;
        yield return new WaitForSeconds(_jumpDelay);

        if (!_isActive || _state != FoxState.PrepareJump) yield break;

        Vector3 safeLandingPos = targetPos + Vector3.up * 1.5f;
        Vector2 jumpVel = CalculateJumpVelocity(transform.position, safeLandingPos);

        _rb.linearVelocity = jumpVel;
        _state = FoxState.Jumping;
    }

    // 바닥 체크 로직 개선 (피벗 위치 상관없이 콜라이더 전체 영역 사용)
    private bool CheckGrounded()
    {
        if (_collider == null)
        {
            _collider = GetComponent<Collider2D>();
            if (_collider == null) return false;
        }

        // 콜라이더의 실제 중심과 크기를 가져옴
        Vector2 center = _collider.bounds.center;
        Vector2 size = _collider.bounds.size;

        // 박스 너비는 살짝 줄여서 벽 비비기 방지
        size.x *= 0.9f;

        // 발 밑으로 0.2f 만큼 쏴서 땅이 있는지 체크
        RaycastHit2D hit = Physics2D.BoxCast(center, size, 0f, Vector2.down, 0.2f, _groundLayer);
        return hit.collider != null;
    }

    private bool IsGapAhead(float dir)
    {
        Vector2 origin = transform.position + new Vector3(dir * 1.5f, 0, 0);
        return !Physics2D.Raycast(origin, Vector2.down, 4.0f, _groundLayer);
    }

    private Vector3? GetIntermediatePlatform(Vector3 finalTarget)
    {
        if (_boss == null || _boss.SpawnPoints == null) return null;
        Vector3 currentPos = transform.position;
        Vector3? bestPoint = null;
        float closestDist = float.MaxValue;
        foreach (Transform point in _boss.SpawnPoints)
        {
            if (point == null) continue;
            Vector3 pos = point.position;
            if (pos.y > currentPos.y + 1.0f && pos.y < finalTarget.y - 0.5f)
            {
                float dist = Vector2.Distance(currentPos, pos);
                if (dist < closestDist) { closestDist = dist; bestPoint = pos + Vector3.up * 1.0f; }
            }
        }
        return bestPoint;
    }

    private void HandleRetreat()
    {
        float dir = (transform.position.x >= 0) ? 1f : -1f;
        _renderer.flipX = (dir < 0);
        float retreatSpeed = _boss.Data.FoxMoveSpeed * 3.0f;
        if (CheckGrounded())
        {
            if (IsGapAhead(dir)) _rb.linearVelocity = new Vector2(dir * retreatSpeed, 12f);
            else _rb.linearVelocity = new Vector2(dir * retreatSpeed, _rb.linearVelocity.y);
        }
        if (Mathf.Abs(transform.position.x) > 30f) { gameObject.SetActive(false); _isActive = false; }
    }

    private Vector2 CalculateJumpVelocity(Vector3 start, Vector3 end)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * _rb.gravityScale);
        float apexY = Mathf.Max(start.y, end.y) + 2.5f;
        float dy = apexY - start.y;
        float vy = Mathf.Sqrt(2 * gravity * dy);
        float tUp = vy / gravity;
        float dyDown = apexY - end.y;
        float tDown = Mathf.Sqrt(2 * dyDown / gravity);
        float totalTime = tUp + tDown;
        float dx = end.x - start.x;
        float vx = dx / totalTime;
        if (Mathf.Abs(dx) < 0.5f) vx = (dx >= 0 ? 1f : -1f) * 2.0f;
        vx = Mathf.Clamp(vx, -15f, 15f);
        return new Vector2(vx, vy);
    }

    private void EnablePhysics(bool enable)
    {
        if (enable)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _collider.enabled = true;
        }
        else
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _collider.enabled = false;
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void TeleportToRandomBook()
    {
        SetTarget(FindRandomBook());
        if (_targetItem != null) transform.position = _targetItem.transform.position + Vector3.up * 1.0f;
        else transform.position = _boss.CenterPoint.position;
    }

    private void UpdateVisuals()
    {
        _renderer.sprite = _normalSprite;
        _renderer.color = _originalColor;
        transform.localScale = Vector3.one;
    }

    private void SetTarget(GimmickItemObject item)
    {
        _targetItem = item;
    }

    private GimmickItemObject FindRandomBook()
    {
        var items = _boss.GetActiveItems();
        if (items == null || items.Count == 0) return null;
        return items[Random.Range(0, items.Count)];
    }

    private GimmickItemObject FindNearestBook()
    {
        var items = _boss.GetActiveItems();
        GimmickItemObject nearest = null;
        float minD = float.MaxValue;
        foreach (var item in items)
        {
            if (item == null) continue;
            float d = Vector2.Distance(transform.position, item.transform.position);
            if (d < minD) { minD = d; nearest = item; }
        }
        return nearest;
    }
    #endregion
}