using UnityEngine;

public class DashAttackState : IState<Player>
{
  
    float _timer;
    Vector2 _dashDir;
    Vector2 _startPos;
    bool _isBounced = false;

    public void Enter(Player _player)
    {
        _player.SetPhysicsMaterial(true);

        _timer = 0f;
        _isBounced = false;
        _startPos = _player.transform.position;

        //방향설정
        float dirX = _player.Spr.flipX ? -1f : 1f;
        _dashDir = new Vector2(dirX, 0f);

        //히트박스 활성화
        if(_player.DashHitbox != null)
        {
            _player.DashHitbox.SetActive(true);
            // 히트박스의 현재 로컬 위치를 가져와서 방향에 따라 x값 반전
            Vector3 hbPos = _player.DashHitbox.transform.localPosition;
            hbPos.x = Mathf.Abs(hbPos.x) * dirX; // 절대값에 방향(1 or -1)을 곱함
            _player.DashHitbox.transform.localPosition = hbPos;
        }

        _player.Rb.gravityScale = 0f;
        _player.Rb.linearVelocity = Vector2.zero;

        _player.Rb.AddForce(_dashDir * _player.DashSpeed, ForceMode2D.Impulse);
    }

    public void Exit(Player _player)
    {
        if(_player.DashHitbox != null)
        {
            _player.DashHitbox.SetActive(false);
        }
        _player.Rb.gravityScale = 1f;
    }

    public void Execute(Player _player)
    {
        if(_isBounced )
        {
            return;
        }

        _timer += Time.deltaTime;

        float movedDistance = Vector2.Distance(_startPos, _player.transform.position);

        // [중요] 거리에 도달하거나 시간이 다 되면 상태 종료
        if (movedDistance >= _player.DashDistance || _timer >= _player.DashDuration)
        {
            _player.SetState(new FallState());
        }
    }

    public void HandleBounce(Player _player,Vector2 hit)
    {
        if( _isBounced )
        {
            return;
        }
        _isBounced = true;

        _player.Rb.linearVelocity = Vector2.zero;
        _player.Rb.gravityScale = 1f;

        // [중요] 벽에 파고든 상태일 수 있으므로 벽 밖으로 살짝 밀어줌 (위치 보정)
        _player.transform.position += (Vector3)hit * 0.2f;

        Vector2 bounceDir = (Vector2.up + hit).normalized;
        _player.Rb.AddForce(bounceDir * _player.BounceForce, ForceMode2D.Impulse);

        _player.SetState(new FallState());
    }
}
