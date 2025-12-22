using UnityEngine;

public class IdleState : IState<Player>
{
    public void Enter(Player _player)
    {
        _player.Anim.SetInteger(_player.animState, 0);
        _player.Anim.SetBool(_player.animFalling, false);

        _player.SetPhysicsMaterial(false);

        Vector2 velocity = _player.Rb.linearVelocity;
        velocity.x = 0f;
        _player.Rb.linearVelocity = velocity;
        _player.Rb.gravityScale = 1f;
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
        if(_player.MoveInput.x != 0)
        {
            _player.SetState(new MoveState());
            return;
        }
        
    }
}
