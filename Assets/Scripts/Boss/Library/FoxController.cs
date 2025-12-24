using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoxController : MonoBehaviour, IDamageable
{
    #region Inspector Fields
    [Header("Prefabs & References")]
    [SerializeField] private GameObject _shadowAttackPrefab;
    [SerializeField] private GameObject _detectEffectPrefab;
    [SerializeField] private Sprite _shadowSprite;
    [SerializeField] private Sprite _normalSprite;

    [Header("Check Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _targetLayer; // 공격 판정용

    [Header("Movement Settings")]
    [Tooltip("점프 전 준비 시간 (초)")]
    [SerializeField] private float _jumpDelay = 0.4f;

    [Header("Attack Settings (Local)")]
    // 데미지나 감지 범위 등은 Boss2Data를 사용하므로 제거됨
    [Tooltip("물기 공격 사거리 (판정 범위)")]
    [SerializeField] private float _biteRange = 2.5f;
    [Tooltip("물기 시 플레이어 고정 위치 오프셋")]
    [SerializeField] private float _holdOffset = 0.8f;

    [Header("Shadow Mode Settings (Local)")]
    // 속도 배율, 데미지 등은 Boss2Data 사용
    [SerializeField] private Vector2 _shadowAttackSize = new Vector2(3.0f, 5.0f);
    [SerializeField] private Vector2 _attackBoxOffset = new Vector2(0f, 2.5f);
    [SerializeField] private float _attackDuration = 1.5f;
    #endregion

    #region Private Fields
    private Stage2Boss _boss;
    private UnityEngine.Transform _player;
    private Collider2D _playerCollider;
    private bool _isActive = false;

    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private Rigidbody2D _rb;

    private enum FoxState
    {
        Idle,
        ChasingBook,
        ChasingPlayer,
        Biting,
        PrepareJump,
        Jumping,
        Eating,
        Retreat,
        ShadowChasing,
        ShadowChargingExplosion
    }
    [SerializeField]
    private FoxState _state = FoxState.Idle;

    private GimmickItemObject _targetItem;
    private Color _originalColor;
    private float _gravityScale = 3f;

    private bool _isShadowMode = false;
    private GameObject _currentAttackInstance;
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

        if (_state == FoxState.Biting || _state == FoxState.ShadowChargingExplosion)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }

        if (_state == FoxState.Retreat)
        {
            HandleRetreat();
            UpdateVisuals();
            return;
        }

        if (_state == FoxState.PrepareJump || _state == FoxState.Eating)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }

        if (_state == FoxState.Jumping)
        {
            if (_rb.linearVelocity.y <= 0 && CheckGrounded())
            {
                if (_isShadowMode)
                {
                    _state = FoxState.ShadowChasing;
                }
                else
                {
                    if (_state == FoxState.ChasingPlayer || CheckPlayerAggro())
                        _state = FoxState.ChasingPlayer;
                    else
                        _state = FoxState.ChasingBook;
                }
            }
        }
        // 모든 추적 상태에서 동일한 MoveToTarget 로직 사용
        else if (_state == FoxState.ChasingBook && _targetItem != null)
        {
            MoveToTarget(_targetItem.transform.position, _boss.Data.FoxMoveSpeed);
        }
        else if (_state == FoxState.ChasingPlayer && _player != null)
        {
            MoveToTarget(_player.position, _boss.Data.FoxMoveSpeed);
        }
        else if (_state == FoxState.ShadowChasing && _player != null)
        {
            // [Fix] Boss2Data 사용
            float shadowSpeed = _boss.Data.FoxMoveSpeed * _boss.Data.FoxShadowSpeedMultiplier;
            MoveToTarget(_player.position, shadowSpeed);
        }

        UpdateVisuals();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_state == FoxState.Retreat && collision.gameObject.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
            _isActive = false;
        }
    }

    private void OnDisable()
    {
        if (_currentAttackInstance != null)
        {
            Destroy(_currentAttackInstance);
            _currentAttackInstance = null;
        }

        if (_playerCollider != null && _collider != null)
        {
            Physics2D.IgnoreCollision(_collider, _playerCollider, false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _biteRange);

        Gizmos.color = Color.yellow;
        // [Fix] _boss가 할당되지 않은 에디터 상태에서는 기본값 8.0f 등으로 표시하거나 예외처리
        float range = (_boss != null) ? _boss.Data.FoxDetectRange : 8.0f;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = new Color(0.5f, 0, 0.5f, 0.5f);
        Vector3 boxCenter = transform.position + (Vector3)_attackBoxOffset;
        Gizmos.DrawWireCube(boxCenter, _shadowAttackSize);
    }
    #endregion

    #region Public Methods
    public void ActivateFox(Stage2Boss boss, UnityEngine.Transform player)
    {
        _boss = boss;
        _isActive = true;
        _player = player;
        _isShadowMode = false;

        if (_player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) _player = playerObj.transform;
        }

        if (_collider == null) _collider = GetComponent<Collider2D>();

        if (_targetLayer.value == 0 && _player != null)
        {
            int playerLayer = _player.gameObject.layer;
            _targetLayer = 1 << playerLayer;
        }

        if (_player != null)
        {
            _playerCollider = _player.GetComponent<Collider2D>();
            if (_playerCollider != null && _collider != null)
            {
                Physics2D.IgnoreCollision(_collider, _playerCollider, true);
            }
        }

        TeleportToRandomBook();

        gameObject.SetActive(true);
        EnablePhysics(true);

        StartCoroutine(AI_RoutineLoop());
    }

    public void ForceRetreat()
    {
        if (!_isActive) return;
        ForceReleasePlayer();
        StopAllCoroutines();
        _state = FoxState.Retreat;
        EnablePhysics(true);
    }

    public void TakeDamage(float damage, float knockback, Vector3 hitPos)
    {
        if (_state == FoxState.Biting)
        {
            Debug.Log("FoxController: 피격당함 -> 그림자 모드로 변신");
            ForceReleasePlayer();
            StartCoroutine(StartShadowModeRoutine());
        }
    }
    #endregion

    #region AI Logic
    private IEnumerator AI_RoutineLoop()
    {
        SetTarget(FindRandomBook());

        while (_isActive)
        {
            if (_isShadowMode)
            {
                yield return null;
                continue;
            }

            if (CheckPlayerAggro())
            {
                ShowDetectEffect();
                yield return StartCoroutine(ChaseAndBiteRoutine());
                continue;
            }

            if (_targetItem == null)
            {
                SetTarget(FindNearestBook());
                yield return null;
                continue;
            }

            _state = FoxState.ChasingBook;
            bool bookReached = false;

            while (_targetItem != null && !bookReached)
            {
                if (_isShadowMode) break;
                if (CheckPlayerAggro())
                {
                    bookReached = false;
                    break;
                }

                float dist = Vector2.Distance(transform.position, _targetItem.transform.position);
                // 도착 판정
                if (dist <= 1.0f && CheckGrounded() && Mathf.Abs(_rb.linearVelocity.y) < 0.1f)
                {
                    bookReached = true;
                }
                yield return null;
            }

            if (_isShadowMode) continue;
            if (!bookReached && CheckPlayerAggro()) continue;

            if (_targetItem != null && bookReached)
            {
                yield return StartCoroutine(EatBookRoutine());
            }
        }
    }

    private IEnumerator StartShadowModeRoutine()
    {
        _isShadowMode = true;
        _state = FoxState.Idle;

        // 그림자 변신 연출
        float t = 0;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(1f, 0.2f, 1f);

        while (t < 0.4f)
        {
            t += Time.deltaTime;
            float ratio = t / 0.4f;
            transform.localScale = Vector3.Lerp(startScale, endScale, ratio);
            _renderer.color = Color.Lerp(_originalColor, Color.black, ratio);
            yield return null;
        }
        transform.localScale = endScale;
        _renderer.color = Color.black;

        yield return new WaitForSeconds(0.2f);

        TeleportToRandomSpawnPoint();

        _state = FoxState.ShadowChasing;
        Debug.Log("FoxController: 그림자 추격 시작");

        while (_isActive && _isShadowMode)
        {
            if (_player == null) break;

            float dist = Vector2.Distance(transform.position, _player.position);

            if (dist <= _biteRange && CheckGrounded())
            {
                yield return StartCoroutine(ShadowAttackRoutine());
                break;
            }

            yield return null;
        }
    }

    private IEnumerator ShadowAttackRoutine()
    {
        _state = FoxState.ShadowChargingExplosion;
        _rb.linearVelocity = Vector2.zero;

        // [Fix] Boss2Data 사용
        float delay = _boss.Data.FoxAoeDelay;
        Debug.Log($"FoxController: 공격 준비. {delay}초 뒤 발동");

        Vector3 spawnPosition = GetGroundPosition(transform.position);

        if (_shadowAttackPrefab != null)
        {
            _currentAttackInstance = Instantiate(_shadowAttackPrefab, spawnPosition, Quaternion.identity);
            SetAlpha(_currentAttackInstance, 0.4f);
        }

        float timer = 0f;
        while (timer < delay)
        {
            timer += Time.deltaTime;
            if (!_isActive) yield break;
            yield return null;
        }

        if (!_isActive) yield break;

        if (_currentAttackInstance != null) Destroy(_currentAttackInstance);

        if (_shadowAttackPrefab != null)
        {
            _currentAttackInstance = Instantiate(_shadowAttackPrefab, spawnPosition, Quaternion.identity);
            SetAlpha(_currentAttackInstance, 1.0f);
        }

        Debug.Log("FoxController: 여우 광역 공격");
        Vector2 attackCenter = (Vector2)spawnPosition + _attackBoxOffset;

        int layerMask = _targetLayer.value != 0 ? _targetLayer.value : -1;
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, _shadowAttackSize, 0f, layerMask);

        foreach (var hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) target = hit.GetComponentInParent<IDamageable>();

            if (target != null)
            {
                // [Fix] Boss2Data 사용
                target.TakeDamage(_boss.Data.FoxAoeDamage, _boss.Data.FoxShadowKnockback, attackCenter);
                Debug.Log($"FoxController: {hit.name}에게 광역 데미지 적용");
            }
        }

        yield return new WaitForSeconds(_attackDuration);

        if (_currentAttackInstance != null)
        {
            Destroy(_currentAttackInstance);
            _currentAttackInstance = null;
        }

        _isShadowMode = false;
        _isActive = false;
        gameObject.SetActive(false);
    }

    private IEnumerator ChaseAndBiteRoutine()
    {
        _state = FoxState.ChasingPlayer;
        Debug.Log("FoxController: 플레이어 추격 시작");

        // [Fix] Boss2Data 사용
        float aggroRange = _boss.Data.FoxDetectRange;

        while (_isActive && _player != null)
        {
            if (_isShadowMode) yield break;

            float dist = Vector2.Distance(transform.position, _player.position);

            if (dist > aggroRange * 1.5f) break;

            bool isStableGrounded = CheckGrounded() && Mathf.Abs(_rb.linearVelocity.y) < 0.1f;

            if (dist <= _biteRange && isStableGrounded)
            {
                yield return StartCoroutine(BiteLogic());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator BiteLogic()
    {
        _state = FoxState.Biting;
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        _collider.enabled = true;
        _collider.isTrigger = true;

        IDamageable targetDamageable = _player.GetComponent<IDamageable>();
        if (targetDamageable == null) targetDamageable = _player.GetComponentInParent<IDamageable>();
        if (targetDamageable == null) targetDamageable = _player.GetComponentInChildren<IDamageable>();

        // [Fix] Boss2Data 사용
        float initialDamage = _boss.Data.FoxBiteDamage;
        float dotDamage = _boss.Data.FoxBiteDotDamage;

        Debug.Log($"FoxController: 물기 시작 (데미지: {initialDamage})");
        if (targetDamageable != null) targetDamageable.TakeDamage(initialDamage, 0, transform.position);

        float timer = 0f;
        float facingDir = _renderer.flipX ? -1f : 1f;

        while (_state == FoxState.Biting && _player != null)
        {
            Vector3 holdPosition = transform.position + new Vector3(facingDir * _holdOffset, 0, 0);
            _player.position = holdPosition;

            yield return null;
            timer += Time.deltaTime;

            if (timer >= 1.0f)
            {
                if (targetDamageable != null) targetDamageable.TakeDamage(dotDamage, 0, transform.position);
                timer = 0f;
            }
        }

        ForceReleasePlayer();
    }

    private IEnumerator EatBookRoutine()
    {
        _state = FoxState.Eating;
        _rb.linearVelocity = Vector2.zero;

        float elapsed = 0f;
        // [Fix] Boss2Data 사용 (기존에 3.0f 하드코딩 되어 있었음)
        float eatDuration = _boss.Data.FoxEatDuration;

        while (elapsed < eatDuration)
        {
            elapsed += Time.deltaTime;

            if (_isShadowMode) yield break;

            if (CheckPlayerAggro())
            {
                _state = FoxState.Idle;
                yield break;
            }

            if (_targetItem == null || Vector2.Distance(transform.position, _targetItem.transform.position) > 1.2f)
            {
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

            if (isCorrect) _state = FoxState.Retreat;
            else
            {
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

        float xDiff = targetPos.x - transform.position.x;
        float yDiff = targetPos.y - transform.position.y;
        float dirX = Mathf.Sign(xDiff);

        // 우회 로직 강화: 목표가 아래에 있으면 수평 거리 상관없이 계속 길을 찾음
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
                // 타겟 아이템이나 플레이어가 아닐 때만 회전
                GameObject targetObj = (_state == FoxState.ChasingPlayer) ? _player.gameObject : (_targetItem != null ? _targetItem.gameObject : null);

                if (!hit.collider.isTrigger && hit.collider.gameObject != targetObj)
                {
                    dirX *= -1f;
                }
            }
        }
        else
        {
            // 일반적인 경우: 가까우면 멈춤 (플레이어 추적 시에는 좀 더 바짝)
            float stopDist = (_state == FoxState.ChasingPlayer) ? 0.1f : 0.2f;
            if (Mathf.Abs(xDiff) < stopDist) dirX = 0;
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

    // 바닥 체크 로직
    private bool CheckGrounded()
    {
        if (_collider == null)
        {
            _collider = GetComponent<Collider2D>();
            if (_collider == null) return false;
        }

        Vector2 center = _collider.bounds.center;
        Vector2 size = _collider.bounds.size;
        size.x *= 0.9f;

        RaycastHit2D hit = Physics2D.BoxCast(center, size, 0f, Vector2.down, 0.2f, _groundLayer);
        return hit.collider != null;
    }

    private bool IsGapAhead(float dir)
    {
        Vector2 origin = transform.position + new Vector3(dir * 1.5f, 0, 0);
        return !Physics2D.Raycast(origin, Vector2.down, 4.0f, _groundLayer);
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
        float apexY = Mathf.Max(start.y, end.y);
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
            _collider.isTrigger = false;
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
        if (_state == FoxState.Jumping || _state == FoxState.PrepareJump)
        {
            transform.localScale = Vector3.one;
            _renderer.sprite = _normalSprite;
            _renderer.color = _originalColor;
        }
        else if (_isShadowMode)
        {
            transform.localScale = new Vector3(1f, 0.2f, 1f);
            _renderer.sprite = _shadowSprite;
            _renderer.color = Color.black;
        }
        else
        {
            transform.localScale = Vector3.one;
            _renderer.sprite = _normalSprite;
            _renderer.color = _originalColor;
        }
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

    // Helper Methods
    private bool CheckPlayerAggro()
    {
        if (_player == null || !_isActive) return false;
        if (_state == FoxState.Retreat || _state == FoxState.Biting || _isShadowMode) return false;

        // [Fix] Boss2Data 사용
        float dist = Vector2.Distance(transform.position, _player.position);
        return dist <= _boss.Data.FoxDetectRange;
    }

    private void ShowDetectEffect()
    {
        if (_detectEffectPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f;
            GameObject effect = Instantiate(_detectEffectPrefab, spawnPos, Quaternion.identity, transform);
            Destroy(effect, 1.5f);
        }
    }

    private void ForceReleasePlayer()
    {
        if (_collider != null)
        {
            _collider.enabled = true;
            _collider.isTrigger = false;
        }

        if (_isActive && _state != FoxState.Retreat)
        {
            EnablePhysics(true);
        }
    }

    private void TeleportToRandomSpawnPoint()
    {
        if (_boss != null && _boss.SpawnPoints != null && _boss.SpawnPoints.Length > 0)
        {
            int rnd = Random.Range(0, _boss.SpawnPoints.Length);
            transform.position = _boss.SpawnPoints[rnd].position;
        }
        else
        {
            TeleportToRandomBook();
        }
    }

    private void SetAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers)
        {
            Color c = r.color;
            c.a = alpha;
            r.color = c;
        }
    }

    private Vector3 GetGroundPosition(Vector3 currentPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(currentPos, Vector2.down, 10f, _groundLayer);
        if (hit.collider != null)
        {
            return hit.point;
        }
        return currentPos;
    }
    #endregion
}