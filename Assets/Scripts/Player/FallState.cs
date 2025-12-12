using UnityEngine;

public class FallState : IPlayerState
{
    Player _player;
    public FallState(Player player)
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
        Debug.Log($"³«ÇÏ½ºÇÇµå: {_player.Rb.linearVelocity.y}");
        if (_player.Rb.linearVelocity.y <= _player.MaxFallSpeedForStun)
        {
            _player.IsStunStarted = true;
        }

        if (_player.IsGrounded)
        {
            if (_player.IsStunStarted)
            {
                _player.IsStunStarted = false;
                _player.SetState(new StunState(_player));
                return;
            }

            if(_player.MoveInput.x != 0)
            {
                _player.SetState(new MoveState(_player));
            }
            else
            {
                _player.SetState(new IdleState(_player));
            }
            return;
        }
    }
}
