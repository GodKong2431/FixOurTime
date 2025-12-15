using UnityEngine;

public class MoveState : IPlayerState
{
    Player _player;
    public MoveState(Player player)
    {
        _player = player;
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


        Vector2 targetVelocity = new Vector2(_moveInput.x * _player.MoveSpeed*_player.CurrentTimeScale, _player.Rb.linearVelocity.y);
        _player.Rb.linearVelocity = Vector2.Lerp(
            _player.Rb.linearVelocity,
            targetVelocity,
            _player.PlayerDeltaTime * _player.AccelerationRate
            );
    }
}
