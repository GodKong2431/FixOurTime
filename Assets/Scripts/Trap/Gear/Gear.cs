using UnityEngine;

public class Gear : TrapBase
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.transform.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(_trapDamage, _knockbackForce, transform.position);
        }
    }
}
