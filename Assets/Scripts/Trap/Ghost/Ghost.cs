using UnityEngine;

public class Ghost : DamageableTrapBase
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if(collision.CompareTag("PlayerAttack"))
        {
            SoundManager.Instance.PlaySFX("SFX_Ghost_ImmuneHit");
        }
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);
        if(collision.CompareTag("PlayerAttack"))
        {
            SoundManager.Instance.PlaySFX("SFX_Ghost_ImmuneHit");
        }
    }
}
