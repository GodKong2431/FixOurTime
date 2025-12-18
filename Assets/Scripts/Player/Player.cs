using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour,IDamageable
{
    [Header("기본 상태 값 설정")]
    IState<Player> _currentState;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _maxHp = 100;
    float _currentHp;
    Vector2 _moveInput;
    public event Action<float, float> OnHpChanged;

    [Header("피직스 마테리얼")]
    [SerializeField] PhysicsMaterial2D _frictionMaterial;  //마찰력1
    [SerializeField] PhysicsMaterial2D _bounceMaterial;   //바운스1

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
    [SerializeField] float _attackCooldown = 1.0f;
    [SerializeField] LayerMask _attackTargetLayer;
    float _nextAttackTime = 0; //다음공격 가능한 시간

    [Header("가속설정")]
    [SerializeField] float _accelerationGravity = 2.0f;
    [SerializeField] float _accelerationScale = 2.0f;
    float _currentTimeScale = 1.0f;
    float _baseTimeScale = 1.0f;
    float _accelerationRate = 10f;
    [SerializeField] bool _isSpeedBoostEnabled = false;
    [SerializeField] SpeedBoostUI _boostUI;
    private Coroutine _speedBoostCoroutine;

    [Header("더블점프 설정")]
    [SerializeField] int _airJumpCount = 1; //공중점프 가능횟수 (1이면 공중에서추가1회라는뜻)
    [SerializeField] float _doubleJumpForce = 5f;
    int _currentAirJump;
    bool _isAirJump = true; //2단점프 가능여부
    [SerializeField] bool _isDoubleJumpEnabled = false;

    [Header("점프 대시공격 설정")]
    [SerializeField] GameObject _dashHitbox;
    [SerializeField] float _dashDistance = 5.0f; //대쉬거리
    [SerializeField] float _dashSpeed = 10f; //대쉬 속도
    [SerializeField] float _dashDuration = 0.5f; //대쉬 적용시간
    [SerializeField] float _bounceForce = 5f; // 튕겨나가는 힘
    [SerializeField] bool _isDashAttackEnabled = false;

    Rigidbody2D _rb;
    SpriteRenderer _spr;

    //프로퍼티
    public int AirJumpCount => _airJumpCount;
    public int CurrentAirJump { get => _currentAirJump; set => _currentAirJump = value; }
    public float MoveSpeed => _moveSpeed;
    public float MinJumpForce => _minJumpForce;
    public float MaxJumpForce => _maxJempForce;
    public float DoubleJumpForce => _doubleJumpForce;
    public float MaxChargeTime => _maxChargeTime;
    public float MaxFallSpeedForStun => _maxFallSpeedForStun;
    public float StunDuration => _stunDuration;
    public float HitDuration => _hitDuration;
    public float AttackDuration => _attackDuration;
    public float AttackRange => _attackRange;
    public float AttackDamage => _attackDamage;
    public float AttackCooldown => _attackCooldown;
    public float AccelerationRate => _accelerationRate;
    public float AccelerationGravity => _accelerationGravity;
    public float DashDistance => _dashDistance;
    public float DashDuration => _dashDuration;
    public float BounceForce => _bounceForce;
    public float DashSpeed => _dashSpeed;
    public float PlayerDeltaTime => Time.deltaTime * _currentTimeScale; //플레이어 전용 델타타임, 가속구현용
    public float CurrentChargeTime { get => _currentChargeTime; set => _currentChargeTime = value; }
    public float CalculatedJumpForce { get => _calculatedJumpForce; set => _calculatedJumpForce = value; }
    public float JumpDirX { get => _jumpDirX; set => _jumpDirX = value; }
    public float CurrentHp { get=>_currentHp; set => _currentHp = value; }
    public float CurrentTimeScale { get =>_currentTimeScale; set => _currentTimeScale = value; }
    public GameObject DashHitbox => _dashHitbox;
    public LayerMask AttackTargetLayer => _attackTargetLayer;
    public Vector2 MoveInput => _moveInput;
    public Rigidbody2D Rb => _rb;
    public SpriteRenderer Spr => _spr;
    public IState<Player> CurrentState => _currentState;
    public bool IsGrounded => _isGrounded;
    public bool IsChargeStarted { get => _isChargeStarted; set => _isChargeStarted = value; }
    public bool IsStunStarted { get => _isStunStarted; set => _isStunStarted = value; }
    public bool IsAirJump { get => _isAirJump; set => _isAirJump = value; }
    public bool IsDoubleJumpEnabled { get => _isDoubleJumpEnabled; set => _isDoubleJumpEnabled = value; }
    public bool IsSpeedBoostEnabled { get => _isSpeedBoostEnabled; set => _isSpeedBoostEnabled = value; }
    public bool CanAttack => Time.time >= _nextAttackTime; //공격 가능한 상태인지 확인용

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
        _currentHp = _maxHp;
        _currentAirJump = _airJumpCount;
        if(_boostUI != null)
        {
            _boostUI.Initialize(this);
        }
        _currentState = new IdleState();
        _currentState.Enter(this);
    }
    private void Update()
    {
        //땅체크
        if(_rb.linearVelocity.y > 0.1f)
        {
            _isGrounded = false;
        }
        else
        {
            _isGrounded = Physics2D.BoxCast(
                _groundChecker.position,    //발사위치
                _groundCheckerSize,
                0f,
                Vector2.down,               //발사방향
                _groundCheckDistance,       //레이저길이
                _groundLayer                //충돌대상체크
                );
        }
            
        _currentState.Execute(this);
    }
    public void SetState(IState<Player> newState)
    {
        if (_currentState == newState) return;
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
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
        if (_currentState is HitState || _currentState is DeadState)
        {
            return;
        }
        //공중에서 점프입력 들어왔을때 2단점프 시도
        if (ctx.started)
        {
            if (_currentState is FallState || _currentState is JumpState)
            {
                if (_currentAirJump > 0 && _isAirJump &&_isDoubleJumpEnabled)
                {
                    _jumpDirX = _moveInput.x; //이거 활성화 시키면 공중에서 더블점프로 방향전환 가능
                    SetState(new JumpState(true));
                    return;
                }
            }
        }
        
        //땅에있을때
        if (_isGrounded)
        {
            if (ctx.started)
            {
                _isChargeStarted = true;
            }
            else if (ctx.canceled)
            {
                if (_currentState is ChargeState currentChargeState)
                {
                    currentChargeState.ReleaseJump(this);
                }
            }
        }
        
    }
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.started && CanAttack)
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
                    SetState(new AttackState());
                }
            }
        }
    }

    public void OnDashAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.started && _isDashAttackEnabled && CanAttack)
        {
            if(_currentState is JumpState ||  _currentState is FallState)
            {
                SetState(new DashAttackState());
            }
        }
    }
    //공격 시작시 쿨타임 적용시키는 메서드
    public void ResetAttackCooldown()
    {
        _nextAttackTime = Time.time + _attackCooldown;
    }
    public void OnSpeedBoost(InputAction.CallbackContext ctx)
    {
        if(!_isSpeedBoostEnabled || _currentState is HitState || _currentState is StunState)
        {
            return;
        }

        if (ctx.started)
        {
            if (_speedBoostCoroutine != null) StopCoroutine(_speedBoostCoroutine);
            _speedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine());
        }
        else if (ctx.canceled)
        {
            StopSpeedBoost();
        }
    }
    private void StopSpeedBoost()
    {
        if (_speedBoostCoroutine != null)
        {
            StopCoroutine(_speedBoostCoroutine);
            _speedBoostCoroutine = null;
        }

        _currentTimeScale = _baseTimeScale;
        _rb.gravityScale = 1f;

        if (_boostUI != null)
        {
            _boostUI.StopBoostIcon();
        }
    }
    private IEnumerator SpeedBoostCoroutine()
    {

        if (_boostUI != null)
        {
            _boostUI.StartBoostIcon();
        }

        _currentTimeScale = _accelerationScale;
        _rb.gravityScale = _accelerationGravity;

        while (true)
        {
            yield return null;
        }
    }

    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {

        if (_currentState is HitState || _currentState is DeadState)
        {
            return;
        }
        StopSpeedBoost();

        _currentHp -= damage;
        _currentHp = Mathf.Max(0f, _currentHp); //0이하로 떨어지기 방지

        OnHpChanged?.Invoke(_currentHp, _maxHp);

        if( _currentHp <= 0)
        {
            SetState(new DeadState());
            return;
        }
        _isAirJump = false;

        float dirX = (transform.position.x > hitPos.x) ? 1f : -1f;

        SetState(new HitState(dirX, KnockbackForce));
    }
    //데이터 저장시키기
    public void SavePlayerState(GameData data)
    {
        data.currentHp = _currentHp;
        data.playerPos = transform.position;
        data.maxHp = _maxHp;
    }
    //데이터 불러오기
    public void LoadPlayerData(GameData data)
    {
        _currentHp = data.currentHp;

        transform.position = data.playerPos;

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Dynamic;

        SetState(new IdleState());
    }

    //체크포인트에서 호출시킬 메서드
    public void CheckPoint()
    {
        GameData data = GameDataManager.Load();
        SavePlayerState(data);
        GameDataManager.Save(data);
    }
    //부활
    public void Respawn()
    {
        GameData data = GameDataManager.Load();

        _currentHp = data.maxHp;
        OnHpChanged?.Invoke(_currentHp, _maxHp);

        transform.position = data.playerPos;

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        SetState(new IdleState());
    }

    //기능 해금들
    public void UnlockDoubleJump()
    {
        _isDoubleJumpEnabled = true;
        Debug.Log("더블 점프 기능이 활성화되었습니다.");
        
    }

    public void UnlockSpeedBoost()
    {
        _isSpeedBoostEnabled = true;
        Debug.Log("가속 기능이 활성화되었습니다.");
    }
    public void UnlockDashAttack()
    {
        _isDashAttackEnabled = true;
        Debug.Log("대시어택 기능이 활성화되었습니다.");
    }
    //마테리얼 교체
    public void SetPhysicsMaterial(bool isBounce)
    {
        _rb.sharedMaterial = isBounce ? _bounceMaterial : _frictionMaterial;
    }

    //땅체크 기즈모 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red; //땅에닿으면 녹, 아니면 빨

        Vector3 boxChecker = (Vector2)_groundChecker.position + Vector2.down * (_groundCheckDistance + _groundCheckerSize.y / 2f);

        Gizmos.DrawWireCube(boxChecker, _groundCheckerSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundChecker.position, 0.05f);
    }
}
