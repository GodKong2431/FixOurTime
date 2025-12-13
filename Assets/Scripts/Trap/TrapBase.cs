using UnityEngine;

public abstract class TrapBase : MonoBehaviour
{
    [Header("트랩 데미지")]
    [SerializeField] protected int _trapDamage;
    [SerializeField] protected float _knockbackForce;
    [SerializeField] protected float _invincibleTime;
}
