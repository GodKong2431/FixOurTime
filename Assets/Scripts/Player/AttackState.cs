using UnityEngine;

public class AttackState : IState<Player>
{
    float _timer;

    public void Enter(Player _player)
    {
        _timer = 0;

        //공격중 이동 못하게
        _player.Rb.linearVelocity = Vector2.zero;

        float dirX = _player.Spr.flipX ? -1f : 1f;

        Vector2 origin = _player.transform.position;
        Vector2 dir = new Vector2(dirX, 0f);

        //그려보기
        Debug.DrawRay(origin, dir * _player.AttackRange, Color.red, _player.AttackDuration);

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, _player.AttackRange, _player.AttackTargetLayer);

        if (hit)
        {
            Debug.Log($"이놈 공격중 : {hit.collider.name}");
            
            IDamageable damageableTarget = hit.collider.GetComponent<IDamageable>();

            if(damageableTarget != null)
            {
                //적에게 넉백 줄거면 이거쓰고, 플레이어에 따로 빼도됨
                float attackKnockbackForce = 5f;

                damageableTarget.TakeDamage(
                    _player.AttackDamage,
                    attackKnockbackForce,
                    _player.transform.position
                    );
            }

        }
    
    }

    public void Exit(Player _player)
    {
        Debug.Log("공격끝");
    }

    public void Execute(Player _player)
    {
        _timer += _player.PlayerDeltaTime;

        if(_timer >= _player.AttackDuration)
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
