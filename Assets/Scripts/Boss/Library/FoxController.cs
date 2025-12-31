using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoxController : MonoBehaviour, IDamageable
{
    #region Inspector Fields
    [Header("Animation")]
    [SerializeField] private FoxAnimationController _animController;

    [Header("Debug Settings")]
    [Tooltip("체크하면 플레이어에게 공격당한 상황을 시뮬레이션 (TakeDamage 호출)")]
    public bool _testHitTrigger = false;

    [Header("Prefabs & References")]
    [SerializeField] private GameObject _shadowAttackPrefab;
    [SerializeField] private GameObject _detectEffectPrefab;

    [Header("Check Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _targetLayer;

    [Header("Movement Settings")]
    [Tooltip("점프 전 준비 시간 (초)")]
    [SerializeField] private float _jumpDelay = 0.4f;

    [Header("Attack Settings (Local)")]
    [Tooltip("물기 공격 사거리 (판정 범위)")]
    [SerializeField] private float _biteRange = 2.5f;
    [Tooltip("물기 시 플레이어 고정 위치 오프셋")]
    [SerializeField] private float _holdOffset = 0.8f;

    [Header("Shadow Mode Settings (Local)")]
    [Tooltip("광역 공격 범위 (가로, 세로)")]
    [SerializeField] private Vector2 _shadowAttackSize = new Vector2(1.5f, 4.0f);

    [Tooltip("공격 히트박스 중심 오프셋")]
    [SerializeField] private Vector2 _attackBoxOffset = new Vector2(0f, 0f);

    [Tooltip("경고 후 튀어나오기 전 딜레이 (초)")]
    [SerializeField] private float _shadowExplosionDelay = 1.0f;

    [Tooltip("땅에서 솟아오르는 애니메이션 시간 (초)")]
    [SerializeField] private float _popUpDuration = 0.75f;

    [Tooltip("공격이 완전히 올라온 후 유지되는 시간 (초)")]
    [SerializeField] private float _attackDuration = 1.0f; 
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
    private float _gravityScale = 3f;

    private bool _isShadowMode = false;
    private GameObject _currentAttackInstance;

    private float _groundedTimer = 0f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        if (_animController == null) _animController = GetComponent<FoxAnimationController>();

        _rb.gravityScale = _gravityScale;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        int absGround = LayerMask.NameToLayer("AbsolutelyGround");
        if (absGround != -1) _groundLayer |= (1 << absGround);

        if (_targetLayer.value == 0)
        {
            int pLayer = LayerMask.NameToLayer("Player");
            if (pLayer != -1) _targetLayer = 1 << pLayer;
        }
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;

        if (CheckGrounded())
        {
            _groundedTimer += Time.deltaTime;
        }
        else
        {
            _groundedTimer = 0f;
        }

        if (_testHitTrigger)
        {
            _testHitTrigger = false;
            TakeDamage(10f, 0f, transform.position);
        }

        if (_state == FoxState.Biting || _state == FoxState.ShadowChargingExplosion)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }

        if (_state == FoxState.Retreat)
        {
            HandleRetreat();
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
            float shadowSpeed = _boss.Data.FoxMoveSpeed * _boss.Data.FoxShadowSpeedMultiplier;
            MoveToTarget(_player.position, shadowSpeed);
        }
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
        float range = (_boss != null) ? _boss.Data.FoxDetectRange : 8.0f;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = new Color(0.5f, 0, 0.5f, 0.5f);

        Vector3 spawnPos = transform.position;
        Vector3 boxCenter = spawnPos + (Vector3)_attackBoxOffset;
        Gizmos.DrawWireCube(boxCenter, _shadowAttackSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.right * (GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 1.0f);
    }
    #endregion

    #region Public Methods
    public void ActivateFox(Stage2Boss boss, UnityEngine.Transform player)
    {
        _boss = boss;
        _isActive = true;
        _player = player;
        _isShadowMode = false;
        _groundedTimer = 0f;

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

        if (_renderer != null) _renderer.enabled = true;
        if (_rb != null) _rb.simulated = true;

        if (_animController != null)
        {
            _animController.SetShadowMode(false);
            _animController.ResetTriggers();
        }

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

                bool isJumping = (_state == FoxState.Jumping || _state == FoxState.PrepareJump);

                if (!isJumping && dist <= 1.5f && CheckGrounded())
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

        if (_animController != null)
        {
            _animController.SetShadowMode(true);
        }

        yield return new WaitForSeconds(0.4f);

        TeleportToRandomSpawnPoint();

        _state = FoxState.ShadowChasing;

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

        if (_animController != null) _animController.TriggerShadowExplosion();

        float delay = _shadowExplosionDelay;

        float clampedX = Mathf.Clamp(transform.position.x, -12f, 4f);
        Vector3 targetPos = new Vector3(clampedX, transform.position.y, 0f);
        Vector3 startPos = targetPos + Vector3.down * 25f;

        // 경고 이펙트
        if (_shadowAttackPrefab != null)
        {
            _currentAttackInstance = Instantiate(_shadowAttackPrefab, targetPos, Quaternion.identity);

            Collider2D col = _currentAttackInstance.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

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

        // 여우 숨기기
        if (_renderer != null) _renderer.enabled = false;
        if (_collider != null) _collider.enabled = false;
        if (_rb != null) _rb.simulated = false;

        if (_currentAttackInstance != null) Destroy(_currentAttackInstance);

        // 실제 공격 이펙트 (솟아오름)
        if (_shadowAttackPrefab != null)
        {
            SoundManager.Instance.PlaySFX("SFX_Boss2_Explosion");
            _currentAttackInstance = Instantiate(_shadowAttackPrefab, startPos, Quaternion.identity);

            Collider2D col = _currentAttackInstance.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = true;
                col.isTrigger = true;
            }

            SetAlpha(_currentAttackInstance, 1.0f);

            float emergeTime = _popUpDuration;
            float t = 0f;

            HashSet<int> damagedTargets = new HashSet<int>();

            int checkLayer = _targetLayer.value;
            if (_player != null) checkLayer |= (1 << _player.gameObject.layer);
            if (checkLayer == 0) checkLayer = LayerMask.GetMask("Player", "Default");

            // [1] 올라오는 동안 판정
            while (t < emergeTime)
            {
                t += Time.deltaTime;
                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t / emergeTime);

                if (_currentAttackInstance != null)
                {
                    _currentAttackInstance.transform.position = currentPos;
                }

                // 이동하는 공격체에 판정 박스 따라가기
                Vector2 attackCenter = (Vector2)currentPos + _attackBoxOffset;
                Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, _shadowAttackSize * 1.1f, 0f, checkLayer);

                foreach (var hit in hits)
                {
                    IDamageable target = hit.GetComponent<IDamageable>();
                    if (target == null) target = hit.GetComponentInParent<IDamageable>();

                    if (target != null)
                    {
                        int id = hit.gameObject.GetInstanceID();
                        if (!damagedTargets.Contains(id))
                        {
                            damagedTargets.Add(id);

                            float dmg = _boss != null ? _boss.Data.FoxAoeDamage : 70f;
                            Debug.Log($"[ShadowAttack] 타격 성공: {hit.name}, 데미지: {dmg}");

                            Vector3 fakeHitPos = hit.transform.position + Vector3.down * 5.0f;
                            target.TakeDamage(dmg, _boss != null ? _boss.Data.FoxShadowKnockback : 10f, fakeHitPos);
                        }
                    }
                }

                yield return null;
            }

            if (_currentAttackInstance != null) _currentAttackInstance.transform.position = targetPos;

            // [2] 올라온 상태에서 일정 시간 유지 
            float stayTimer = 0f;
            while (stayTimer < _attackDuration)
            {
                stayTimer += Time.deltaTime;

                // 유지되는 동안에도 판정 박스는 계속 작동 (뒤늦게 들어온 플레이어 피격)
                if (_currentAttackInstance != null)
                {
                    Vector2 attackCenter = (Vector2)_currentAttackInstance.transform.position + _attackBoxOffset;
                    Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, _shadowAttackSize * 1.1f, 0f, checkLayer);

                    foreach (var hit in hits)
                    {
                        IDamageable target = hit.GetComponent<IDamageable>();
                        if (target == null) target = hit.GetComponentInParent<IDamageable>();

                        if (target != null)
                        {
                            int id = hit.gameObject.GetInstanceID();
                            // damagedTargets를 공유하므로 이미 맞은 플레이어는 다시 맞지 않음
                            if (!damagedTargets.Contains(id))
                            {
                                damagedTargets.Add(id);

                                float dmg = _boss != null ? _boss.Data.FoxAoeDamage : 70f;
                                Vector3 fakeHitPos = hit.transform.position + Vector3.down * 5.0f;
                                target.TakeDamage(dmg, _boss != null ? _boss.Data.FoxShadowKnockback : 10f, fakeHitPos);
                            }
                        }
                    }
                }
                yield return null;
            }
        }

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
        float aggroRange = _boss.Data.FoxDetectRange;

        while (_isActive && _player != null)
        {
            if (_isShadowMode) yield break;

            float dist = Vector2.Distance(transform.position, _player.position);

            if (dist > aggroRange * 1.5f) break;

            bool isStableGrounded = CheckGrounded() && Mathf.Abs(_rb.linearVelocity.y) < 1.0f;

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

        if (_animController != null) _animController.TriggerBite();

        _collider.enabled = true;
        _collider.isTrigger = true;

        IDamageable targetDamageable = _player.GetComponent<IDamageable>();
        if (targetDamageable == null) targetDamageable = _player.GetComponentInParent<IDamageable>();
        if (targetDamageable == null) targetDamageable = _player.GetComponentInChildren<IDamageable>();

        float initialDamage = _boss.Data.FoxBiteDamage;
        float dotDamage = _boss.Data.FoxBiteDotDamage;

        if (targetDamageable != null) targetDamageable.TakeDamage(initialDamage, 0, transform.position);

        float timer = 0f;
        float facingDir = _renderer.flipX ? -1f : 1f;

        while (_state == FoxState.Biting && _player != null)
        {
            SoundManager.Instance.PlaySFX("SFX_Boss2_Bite");

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

        if (_animController != null) _animController.SetEating(true);

        float elapsed = 0f;
        float eatDuration = _boss.Data.FoxEatDuration;

        while (elapsed < eatDuration)
        {
            SoundManager.Instance.PlaySFX("SFX_Boss2_BookBite");
            elapsed += Time.deltaTime;

            if (_isShadowMode) break;

            if (CheckPlayerAggro())
            {
                _state = FoxState.Idle;
                break;
            }

            if (_targetItem == null || Vector2.Distance(transform.position, _targetItem.transform.position) > 2.0f)
            {
                _state = FoxState.ChasingBook;
                break;
            }
            yield return null;
        }

        if (_animController != null) _animController.SetEating(false);

        if (_targetItem != null && _state == FoxState.Eating)
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

    private void MoveToTarget(Vector3 targetPos, float speed)
    {
        if (!CheckGrounded()) return;

        float xDiff = targetPos.x - transform.position.x;
        float yDiff = targetPos.y - transform.position.y;
        float dirX = Mathf.Sign(xDiff);

        int obstacleMask = _groundLayer | _wallLayer;

        bool isTargetBelow = (yDiff < -1.0f);

        if (isTargetBelow)
        {
            float currentFacing = _renderer.flipX ? -1f : 1f;
            dirX = currentFacing;

            Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, 1.0f, obstacleMask);

            if (hit.collider != null)
            {
                GameObject targetObj = (_state == FoxState.ChasingPlayer) ? _player.gameObject : (_targetItem != null ? _targetItem.gameObject : null);

                if (!hit.collider.isTrigger && hit.collider.gameObject != targetObj)
                {
                    dirX *= -1f;
                }
            }
            else if (IsGapAhead(dirX))
            {
                RaycastHit2D hitAcross = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, 2.5f, obstacleMask);
                if (hitAcross.collider != null && !hitAcross.collider.isTrigger)
                {
                    dirX *= -1f;
                }
            }
        }
        else
        {
            float stopDist = (_state == FoxState.ChasingPlayer) ? 0.1f : 0.2f;
            if (Mathf.Abs(xDiff) < stopDist) dirX = 0;
        }

        if (dirX != 0)
        {
            bool isLeft = (dirX < 0);
            if (_animController != null) _animController.Flip(isLeft);
            else _renderer.flipX = isLeft;
        }

        bool isHighTarget = yDiff > 0.6f;
        bool isGapAhead = IsGapAhead(dirX);

        Vector2 checkOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
        RaycastHit2D wallHit = Physics2D.Raycast(checkOrigin, Vector2.right * dirX, 1.0f, obstacleMask);
        bool isWallAhead = (wallHit.collider != null && !wallHit.collider.isTrigger);

        bool needJump = isHighTarget || (isGapAhead && !isTargetBelow) || (isWallAhead && !isTargetBelow);

        if (needJump && _groundedTimer >= 1.0f)
        {
            if (isHighTarget && Mathf.Abs(xDiff) > 2.5f && !isGapAhead && !isWallAhead)
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
            _rb.linearVelocity = new Vector2(dirX * speed, _rb.linearVelocity.y);
        }
    }

    private IEnumerator JumpRoutine(Vector3 targetPos)
    {
        _state = FoxState.PrepareJump;

        if (_animController != null) _animController.TriggerJump();

        yield return new WaitForSeconds(_jumpDelay);

        if (!_isActive || _state != FoxState.PrepareJump) yield break;

        Vector3 safeLandingPos = targetPos + Vector3.up * 1.5f;
        Vector2 jumpVel = CalculateJumpVelocity(transform.position, safeLandingPos);

        _rb.linearVelocity = jumpVel;
        _state = FoxState.Jumping;
    }

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

        if (_animController != null) _animController.Flip(dir < 0);
        else _renderer.flipX = (dir < 0);

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
        float apexY = Mathf.Max(start.y, end.y) + 0.5f;
        float dy = apexY - start.y;
        float vy = Mathf.Sqrt(2 * gravity * dy);
        float tUp = vy / gravity;
        float dyDown = apexY - end.y;
        if (dyDown < 0) dyDown = 0;
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

    private bool CheckPlayerAggro()
    {
        if (_player == null || !_isActive) return false;
        if (_state == FoxState.Retreat || _state == FoxState.Biting || _isShadowMode) return false;
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