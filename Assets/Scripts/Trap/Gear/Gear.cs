using UnityEngine;

public class Gear : DamageableTrapBase
{
    [Header("회전 속도")]
    [SerializeField] protected float _rotateSpeed = 180f;

    private void FixedUpdate()
    {
        transform.Rotate(0f, 0f, _rotateSpeed * Time.fixedDeltaTime);
    }
} 
