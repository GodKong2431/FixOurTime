using UnityEngine;

public class AngelCore : MonoBehaviour, IDamageable
{
    [SerializeField] private Stage3AngelBoss _boss;

    public void TakeDamage(float damage, float knockback, Vector3 hitPos)
    {
        if (_boss != null)
        {
            // 보스 본체에게 데미지 전달
            _boss.ApplyDamage(damage);
        }
    }
}