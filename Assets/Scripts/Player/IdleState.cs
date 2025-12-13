using UnityEngine;

public class IdleState : IPlayerState
{
    Player _player;
    public IdleState(Player player)
    {
        _player = player;
    }
    public void Enter()
    {
        Vector2 velocity = _player.Rb.linearVelocity;
        velocity.x = 0f;
        _player.Rb.linearVelocity = velocity;
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
        if(_player.MoveInput.x != 0)
        {
            _player.SetState(new MoveState(_player));
            return;
        }
        
    }
}
