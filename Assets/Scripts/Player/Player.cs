using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour,IDamageable
{
    [Header("기본 상태 값 설정")]
    IPlayerState _currentState;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _maxHp = 100;
    float _currentHp;
    Vector2 _moveInput;

    [Header("땅체크")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundChecker;
    [SerializeField] float _groundCheckDistance = 0.1f;
    [SerializeField] Vector2 _groundCheckerSize = new Vector2(0.8f, 0.1f);
    bool _isGrounded;

    [Header("차지 점프 설정")]
    [SerializeField] float _minJumpForce = 5f;
    [SerializeField] float _maxJempForce = 15f;
    [SerializeField] float _maxChargeTime = 1f;  //최대 충전시간
    float _currentChargeTime;
    float _calculatedJumpForce;
    float _jumpDirX;
    bool _isChargeStarted;

    [Header("낙하 및 스턴 설정")]
    [SerializeField] float _maxFallSpeedForStun = -15f;  //스턴에 빠지는 최대 하강속도
    [SerializeField] float _stunDuration = 1.0f;
    bool _isStunStarted;

    [Header("넉백 설정")]
    [SerializeField] float _hitDuration = 0.5f;

    [Header("공격 설정")]
    [SerializeField] float _attackDuration = 0.3f;
    [SerializeField] float _attackRange = 1.0f;
    [SerializeField] float _attackDamage = 10f;
    [SerializeField]
    LayerMask _attackTargetLayer;

    [Header("가속설정")]
    [SerializeField] float _baseTimeScale = 1.0f;
    [SerializeField] float _currentTimeScale = 1.0f;
    [SerializeField] float _accelerationRate = 10f;
    [SerializeField] float _accelerationGravity = 2.0f;

    //[Header("무적시간")]
    //[SerializeField] float _invincibleDuration = 0.5f;
    //bool _isInvincible;
    //float _invincibleTimer;
   

    Rigidbody2D _rb;
    SpriteRenderer _spr;

    //프로퍼티
    public float MoveSpeed => _moveSpeed;
    public float MinJumpForce => _minJumpForce;
    public float MaxJumpForce => _maxJempForce;
    public float MaxChargeTime => _maxChargeTime;
    public float MaxFallSpeedForStun => _maxFallSpeedForStun;
    public float StunDuration => _stunDuration;
    public float HitDuration => _hitDuration;
    public float AttackDuration => _attackDuration;
    public float AttackRange => _attackRange;
    public float AttackDamage => _attackDamage;
    public float AccelerationRate => _accelerationRate;
    public float AccelerationGravity => _accelerationGravity;
    public float PlayerDeltaTime => Time.deltaTime * _currentTimeScale; //플레이어 전용 델타타임 가속구현용
    //public float InvincibleDuration => _invincibleDuration;
    //public float InvincibleTimer { get => _invincibleTimer; set => _invincibleTimer = value; }
    public float CurrentChargeTime { get => _currentChargeTime; set => _currentChargeTime = value; }
    public float CalculatedJumpForce { get => _calculatedJumpForce; set => _calculatedJumpForce = value; }
    public float JumpDirX { get => _jumpDirX; set => _jumpDirX = value; }
    public float CurrentHp { get=>_currentHp; set => _currentHp = value; }
    public float CurrentTimeScale { get =>_currentTimeScale; set => _currentTimeScale = value; }
    public LayerMask AttackTargetLayer => _attackTargetLayer;
    public Vector2 MoveInput => _moveInput;
    public Rigidbody2D Rb => _rb;
    public SpriteRenderer Spr => _spr;
    public bool IsGrounded => _isGrounded;
    public bool IsChargeStarted { get => _isChargeStarted; set => _isChargeStarted = value; }
    public bool IsStunStarted { get => _isStunStarted; set => _isStunStarted = value; }
    //public bool IsInvincible { get => _isInvincible; set => _isInvincible = value; }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red; //땅에닿으면 녹, 아니면 빨

        Vector3 boxChecker = (Vector2)_groundChecker.position + Vector2.down * (_groundCheckDistance + _groundCheckerSize.y / 2f);

        Gizmos.DrawWireCube(boxChecker, _groundCheckerSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundChecker.position, 0.05f);
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
        _currentHp = _maxHp;
        SetState(new IdleState(this));
    }
    private void Update()
    {
        //땅체크
        _isGrounded = Physics2D.BoxCast(
            _groundChecker.position,    //발사위치
            _groundCheckerSize,
            0f,
            Vector2.down,               //발사방향
            _groundCheckDistance,       //레이저길이
            _groundLayer                //충돌대상체크
            );
        //무적 타이머 업데이트
        //if (_isInvincible)
        //{
        //    _invincibleTimer -= Time.deltaTime;
        //    if (_invincibleTimer <= 0f)
        //    {
        //        _isInvincible = false;
        //    }
        //}

        _currentState.Update();
    }
    public void SetState(IPlayerState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (_currentState is HitState || _currentState is DeadState)
        {
            _moveInput = Vector2.zero;
            return;
        }
        _moveInput = ctx.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (_currentState is HitState || _currentState is DeadState || _currentState is FallState)
        {
            return;
        }
        if (ctx.started)
        {
            _isChargeStarted = true;
        }
        else if (ctx.canceled)
        {
            if(_currentState is ChargeState currentChargeState)
            {
                currentChargeState.ReleaseJump();
            }
        }
    }
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
        {
            //공격 허용 안하는 상태들
            bool _isAttackPrevent = _currentState is HitState ||
                                 _currentState is StunState ||
                                 _currentState is ChargeState ||
                                 _currentState is JumpState ||
                                 _currentState is FallState;

            if (!_isAttackPrevent)
            {
                if (!(_currentState is AttackState))
                {
                    SetState(new AttackState(this));
                }
            }
        }
    }

    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        //if (_isInvincible)
        //{
        //    return;
        //}

        if (_currentState is HitState || _currentState is DeadState)
        {
            return;
        }

        _currentHp -= damage;

        if( _currentHp <= 0)
        {
            SetState(new DeadState(this));
            return;
        }

        float dirX = (transform.position.x > hitPos.x) ? 1f : -1f;

        SetState(new HitState(this, dirX, KnockbackForce));
    }
    public void SavePlayerState(GameData data)
    {
        data.currentHp = _currentHp;
        data.playerPos = transform.position;
        data.maxHp = _maxHp;
    }
    public void LoadPlayerData(GameData data)
    {
        _currentHp = data.currentHp;

        transform.position = data.playerPos;

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Dynamic;

        SetState(new IdleState(this));
    }

    //체크포인트에서 호출시킬 메서드
    public void CheckPoint()
    {
        GameData data = GameDataManager.Load();
        SavePlayerState(data);
        GameDataManager.Save(data);
    }

    public void Respawn()
    {
        GameData data = GameDataManager.Load();

        _currentHp = data.maxHp;

        transform.position = data.playerPos;

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        SetState(new IdleState(this));
    }
}
