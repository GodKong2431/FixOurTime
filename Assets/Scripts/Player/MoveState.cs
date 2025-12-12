using UnityEngine;

public class MoveState : IPlayerState
{
    Player _player;
    float _moveSpeed;
    public MoveState(Player player)
    {
        _player = player;
        _moveSpeed = player.MoveSpeed;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (!_player.IsGrounded)
        {
            _player.SetState(new FallState(_player));
            return;
        }
        if (_player.IsGrounded && _player.IsChargeStarted)
        {
            _player.SetState(new ChargeState(_player));
            return;
        }
        if (_player.MoveInput.x == 0)
        {
            _player.SetState(new IdleState(_player));
            return;
        }


        // 이동 구현
        Vector2 _moveInput = _player.MoveInput;
        if (_moveInput.x > 0)
        {
            _player.Spr.flipX = false;
        }
        else if (_moveInput.x < 0)
        {
            _player.Spr.flipX = true;
        }
        Vector2 _velocity = _player.Rb.linearVelocity;
        _velocity.x = _moveInput.x * _moveSpeed;
        _player.Rb.linearVelocity = _velocity;
    }
}
