using UnityEngine;

public class AttackState : IState<Player>
{
    float _timer;

    public void Enter(Player _player)
    {
        _player.ResetAttackCooldown();

        _player.Anim.SetTrigger(_player.animAttack);

        _timer = 0;

        //공격중 이동 못하게
        _player.Rb.linearVelocity = Vector2.zero;
    }

    public void Exit(Player _player)
    {
    }

    public void Execute(Player _player)
    {
        _timer += _player.PlayerDeltaTime;

        float progress = Mathf.Clamp01(_timer / _player.AttackDuration);
        _player.UpdateAttackProgress(progress);

        if (_timer >= _player.AttackDuration)
        {
            if (_player.IsGrounded)
            {
                if(_player.MoveInput.x != 0)
                {
                    _player.SetState(new MoveState());
                }
                else
                {
                    _player.SetState(new IdleState());
                }
            }
            else
            {
                _player.SetState(new FallState());
            }
        }
    }
}
