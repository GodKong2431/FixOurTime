using UnityEngine;

public class FallState : IState<Player>
{
    public void Enter(Player _player)
    {
        _player.SetPhysicsMaterial(true);

        //Debug.Log("폴 진입");
        if(_player.CurrentTimeScale <= 1.0f)
        {
            _player.Rb.gravityScale =3f;
        }
        else
        {
            _player.Rb.gravityScale = _player.AccelerationGravity;
        }
       
    }

    public void Exit(Player _player)
    {
        //Debug.Log("폴 나감");
        _player.Rb.gravityScale = 1f;
        _player.IsChargeStarted = false;
    }

    public void Execute(Player _player)
    {
        //Debug.Log($"낙하스피드: {_player.Rb.linearVelocity.y}");
        if (_player.Rb.linearVelocity.y <= _player.MaxFallSpeedForStun)
        {
            _player.IsStunStarted = true;
        }

        if (_player.IsGrounded)
        {
            _player.SetPhysicsMaterial(false);

            _player.CurrentAirJump = _player.AirJumpCount;
            _player.IsAirJump = true;

            if (_player.IsStunStarted)
            {
                _player.IsStunStarted = false;
                _player.SetState(new StunState());
                return;
            }

            if(_player.MoveInput.x != 0)
            {
                _player.SetState(new MoveState());
            }
            else
            {
                _player.SetState(new IdleState());
            }
            return;
        }
    }
}
