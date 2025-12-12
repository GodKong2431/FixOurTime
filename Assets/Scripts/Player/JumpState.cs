using UnityEngine;

public class JumpState : IPlayerState
{
    Player _player;
    float _jumpDirX;
    public JumpState(Player player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("점프 진입");
        Vector2 velocity = _player.Rb.linearVelocity;
        //수직속도
        velocity.y = _player.CalculatedJumpForce;
        //수평속도
        _jumpDirX=_player.JumpDirX;

        velocity.x = _jumpDirX * _player.MoveSpeed;

        _player.Rb.linearVelocity = velocity;

        if(_jumpDirX > 0)
        {
            _player.Spr.flipX = false;
        }
        else if( _jumpDirX < 0)
        {
            _player.Spr.flipX = true;
        }
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (_player.Rb.linearVelocity.y <= 0f && !_player.IsGrounded)
        {
            _player.SetState(new FallState(_player));
            return;
        }

        if (_player.IsGrounded && _player.Rb.linearVelocity.y <= 0.01f)
        {
            
            //입력값있으면 무브,아니면 대기상태로 변경
            if (_jumpDirX != 0)
            {
                _player.SetState(new MoveState(_player));
            }
            else
            {
                _player.SetState(new IdleState(_player));
            }
        }
    }
}
