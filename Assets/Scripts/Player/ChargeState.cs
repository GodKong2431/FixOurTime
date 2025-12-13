using UnityEngine;

public class ChargeState : IPlayerState
{
    Player _player;
    public ChargeState(Player player)
    {
        _player = player;
    }
    public void Enter()
    {
        _player.CurrentChargeTime = 0;
        _player.CalculatedJumpForce = _player.MinJumpForce;

        //차지시 이동금지
        Vector2 velocity = _player.Rb.linearVelocity;
        velocity.x = 0f;
        _player.Rb.linearVelocity = velocity;

    }

    public void Exit()
    {
        _player.IsChargeStarted = false;
    }

    public void Update()
    {
        _player.CurrentChargeTime += Time.deltaTime;

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
    public void ReleaseJump()
    {
        _player.SetState(new JumpState(_player));
    }
}
