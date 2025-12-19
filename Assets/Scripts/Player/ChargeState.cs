using UnityEngine;

public class ChargeState : IState<Player>
{
    public void Enter(Player _player)
    {
        _player.SetPhysicsMaterial(false);

        _player.Anim.SetInteger(_player.animState, 2);

        _player.CurrentChargeTime = 0;
        _player.CalculatedJumpForce = _player.MinJumpForce;

        //차지시 이동금지
        Vector2 velocity = _player.Rb.linearVelocity;
        velocity.x = 0f;
        _player.Rb.linearVelocity = velocity;

    }

    public void Exit(Player _player)
    {
        _player.IsChargeStarted = false;
    }

    public void Execute(Player _player)
    {
        if (!_player.IsGrounded)
        {
            _player.SetState(new FallState());
            return;
        }

        //차지 시간 계산
        _player.CurrentChargeTime += _player.PlayerDeltaTime;

        float chargeRatio = Mathf.Clamp01(_player.CurrentChargeTime / _player.MaxChargeTime);

        _player.CalculatedJumpForce = Mathf.Lerp(_player.MinJumpForce,_player.MaxJumpForce,chargeRatio);

        //충전중 방향 입력감지
        Vector2 moveInput = _player.MoveInput;

        _player.JumpDirX = moveInput.x;

        if (moveInput.x > 0)
        {
            _player.Spr.flipX = false;
        }
        else if(moveInput.x < 0)
        {
            _player.Spr.flipX = true;
        }
    }
    public void ReleaseJump(Player _player)
    {
        if (_player.IsGrounded)
        {
            _player.SetState(new JumpState(false));
        }
    }
}
