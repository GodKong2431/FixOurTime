using UnityEngine;

public class AttackState : IPlayerState
{
    Player _player;
    float _timer;

    public AttackState(Player player)
    {
        _player = player;
    }
    public void Enter()
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
            //여기에 데미지 입히는거 구현
            // 인터페이스같은거 구현해서 겟컴퍼넌트 하면 좋을듯?
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

    public void Exit()
    {
        Debug.Log("공격끝");
    }

    public void Update()
    {
        _timer += Time.deltaTime;

        if(_timer >= _player.AttackDuration)
        {
            if (_player.IsGrounded)
            {
                if(_player.MoveInput.x != 0)
                {
                    _player.SetState(new MoveState(_player));
                }
                else
                {
                    _player.SetState(new IdleState(_player));
                }
            }
            else
            {
                _player.SetState(new FallState(_player));
            }
        }
    }
}
