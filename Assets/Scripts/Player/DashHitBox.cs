using System;
using UnityEngine;

public class DashHitBox : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_player.CurrentState is DashAttackState dashState)
        {
            // 벽 혹은 적 레이어 체크
            bool isTarget = ((1 << other.gameObject.layer) & _player.AttackTargetLayer) != 0;
            bool isGround = ((1 << other.gameObject.layer) & LayerMask.GetMask("Ground")) != 0;

            if (isTarget || isGround)
            {
                // 적이라면 데미지 처리 로직 추가 가능
                if (isTarget)
                {
                    other.GetComponent<IDamageable>()?.TakeDamage(_player.AttackDamage, _player.BounceForce, transform.position);
                }

                // 충돌 지점 계산 (간단하게 물체 사이의 방향)
                Vector2 hitNormal = (transform.position - other.transform.position).normalized;

                // 대시 상태에 충돌 알림 (HandleBounce 내부에서 SetState를 호출하므로 즉시 종료됨)
                dashState.HandleBounce(hitNormal);
            }
        }
    }

}
