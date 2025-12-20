using UnityEngine;

public class Fire : DamageableTrapBase
{
    private void OnEnable()
    {
        Destroy(gameObject, 3f);
    }
}