using UnityEngine;

public class JumpState : IPlayerState
{
    Player _player;
    float _jumpDirX;
    bool _isDoubleJump;
    public JumpState(Player player)
    {
        _player = player;
    }
    public JumpState(Player player, bool isDoubleJump)
    {
        _player = player;
        _isDoubleJump = isDoubleJump;
    }

    public void Enter()
    {
        Debug.Log("점프 진입");
        Vector2 velocity = _player.Rb.linearVelocity;
        
        float currentJumpForce;
        float targetGravity = 1f;

        if (_isDoubleJump)
        {
            currentJumpForce = _player.MinJumpForce;
            _player.CurrentAirJump--;
            velocity.y = 0f;
        }
        else
        {
            currentJumpForce = _player.CalculatedJumpForce;
        }

        //가속상태 진입하면 가속중력적용
        if (_player.CurrentTimeScale > 1.0f)
        {
            targetGravity = _player.AccelerationGravity;
        }
        else //가속아닐떄는 그냥 2배중력 적용
        {
            targetGravity = 2f;
        }

        // 점프속력은 높이지만 점력은 안높아지게 설정
        if(targetGravity > 1f)
        {
            float gravityRatio = targetGravity / 1f;

            // V2 = V1 * sqrt(G2/G1) 공식을 사용하여 힘을 보정
            float boostFactor = Mathf.Sqrt(gravityRatio);

            currentJumpForce *= boostFactor;
        }

        velocity.y = currentJumpForce;

        _player.Rb.gravityScale = targetGravity;
            //수평속도
        _jumpDirX = _player.JumpDirX;

        velocity.x = _jumpDirX * _player.MoveSpeed*_player.CurrentTimeScale;

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
        _player.Rb.gravityScale = 1f;
    }

    public void Update()
    {
        if (_player.IsGrounded && _player.Rb.linearVelocity.y <= 0.01f)
        {
            _player.CurrentAirJump = _player.AirJumpCount;
            _player.IsAirJump = true;
            
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
        else if(_player.Rb.linearVelocity.y <= 0f && !_player.IsGrounded)
        {
            _player.SetState(new FallState(_player));
            return;
        }
    }
}
