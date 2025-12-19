using UnityEngine;

public class BossBody : MonoBehaviour
{
    [SerializeField] private Stage1Boss _boss;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어인지 확인
        if (collision.gameObject.TryGetComponent(out IDamageable target))
        {
            // 충돌 지점 계산
            Vector2 hitPoint = collision.GetContact(0).point;

            float dmg = _boss != null ? _boss.BossData.BossBodyContactDamage : 100f;

            // 데미지 100 (즉사), 넉백 일단 없음
            target.TakeDamage(100f, 0f, hitPoint);
        }
    }
}
