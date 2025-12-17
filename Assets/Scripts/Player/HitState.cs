using UnityEngine;

public class HitState : IState<Player>
{
    float _timer;
    float _knockbackDirX;
    float _knockbackForce;
    public HitState(float directionX, float force)
    {
        _knockbackDirX = directionX;
        _knockbackForce = force;
    }
    public void Enter(Player _player)
    {
        Debug.Log("히트 진입");
        _timer = 0f;

        _player.IsChargeStarted = false;
        _player.CurrentChargeTime = 0f;

        Vector2 knockbackVector = new Vector2(
            _knockbackDirX * _knockbackForce,
            _knockbackForce);
        _player.Rb.AddForce(knockbackVector, ForceMode2D.Impulse);
        _player.Spr.color = Color.red;

        _player.Rb.gravityScale = _player.AccelerationGravity;
    }

    public void Exit(Player _player)
    {
        Debug.Log("히트 나감");
        _player.Spr.color = Color.white;
        _player.Rb.gravityScale = 1f;
    }

    public void Execute(Player _player)
    {
        _timer += Time.deltaTime;

        if(_timer >= _player.HitDuration)
        {

            if (_player.IsGrounded)
            {
                _player.Rb.linearVelocity = Vector2.zero;
                _player.SetState(new IdleState());
            }
            else
            {
                _player.SetState(new FallState());
            }
            return;
        }
    }
}
