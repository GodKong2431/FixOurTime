using UnityEngine;

public abstract class DamageableTrapBase : MonoBehaviour
{
    [Header("Æ®·¦ µ¥¹ÌÁö")]
    [SerializeField] protected int _trapDamage = 20;
    [SerializeField] protected float _knockbackForce = 7;
    [SerializeField] protected float _invincibleTime = 0.5f;

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Player player))
        {
            player.TakeDamage(_trapDamage, _knockbackForce, transform.position);
        }
    }
}
