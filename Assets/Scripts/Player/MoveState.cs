using UnityEngine;

public class MoveState : IState<Player>
{

    public void Enter(Player _player)
    {
        _player.SetPhysicsMaterial(false);

        _player.Anim.SetInteger(_player.animState, 1);
    }

    public void Exit(Player _player)
    {
        
    }

    public void Execute(Player _player)
    {
        if (!_player.IsGrounded)
        {
            _player.SetState(new FallState());
            return;
        }
        if (_player.IsGrounded && _player.IsChargeStarted)
        {
            _player.SetState(new ChargeState());
            return;
        }
        if (_player.MoveInput.x == 0)
        {
            _player.SetState(new IdleState());
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
