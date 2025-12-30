using UnityEngine;

public class Fire : DamageableTrapBase
{
    private void OnEnable()
    {
        SoundManager.Instance.PlaySFX("SFX_Boss3_Burning");
        Destroy(gameObject, 3f);
    }
}