using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos);
}
