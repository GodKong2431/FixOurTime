using UnityEngine;

public class BossBody : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어인지 확인
        if (collision.gameObject.TryGetComponent(out IDamageable target))
        {
            // 충돌 지점 계산
            Vector2 hitPoint = collision.GetContact(0).point;

            // 데미지 100 (즉사), 넉백 10f (강하게 밀쳐냄)
            target.TakeDamage(100f, 10f, hitPoint);
        }
    }
}
