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
        Debug.Log("폴 진입");
        if(_player.CurrentTimeScale <= 1.0f)
        {
            _player.Rb.gravityScale =3f;
        }
        else
        {
            _player.Rb.gravityScale = _player.AccelerationGravity;
        }
       
    }

    public void Exit()
    {
        Debug.Log("폴 나감");
        _player.Rb.gravityScale = 1f;
        _player.IsChargeStarted = false;
    }

    public void Update()
    {
        Debug.Log($"낙하스피드: {_player.Rb.linearVelocity.y}");
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
