using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("상태 값 설정")]
    IPlayerState _currentState;
    [SerializeField] float _moveSpeed = 5f;
    Vector2 _moveInput;

    [Header("땅체크")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundChecker;
    [SerializeField] float _groundCheckDistance = 0.1f;
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

    [Header("넉백설정")]
    [SerializeField] float _hitDuration = 0.5f;
   

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
    public float CurrentChargeTime { get => _currentChargeTime; set => _currentChargeTime = value; }
    public float CalculatedJumpForce { get => _calculatedJumpForce; set => _calculatedJumpForce = value; }
    public float JumpDirX { get => _jumpDirX; set => _jumpDirX = value; }
    public Vector2 MoveInput => _moveInput;
    public Rigidbody2D Rb => _rb;
    public SpriteRenderer Spr => _spr;
    public bool IsGrounded => _isGrounded;
    public bool IsChargeStarted { get => _isChargeStarted; set => _isChargeStarted = value; }
    public bool IsStunStarted { get => _isStunStarted; set => _isStunStarted = value; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
        SetState(new IdleState(this));
    }
    private void Update()
    {
        _isGrounded = Physics2D.Raycast(
            _groundChecker.position,    //발사위치
            Vector2.down,               //발사방향
            _groundCheckDistance,       //레이저길이
            _groundLayer                //충돌대상체크
            );
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
        _moveInput = ctx.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
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
    public void TakeDamage(int damage,float knockbackForce, Vector3 hitPos )
    {
        if(_currentState is HitState)
        {
            return;
        }

        float dirX = (transform.position.x > hitPos.x) ? 1f : -1f;

        SetState(new HitState(this, dirX, knockbackForce));
    }
}
