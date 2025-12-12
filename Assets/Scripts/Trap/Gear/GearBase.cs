using UnityEngine;

public class GearBase : TrapBase
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("플레이어 접촉");
        //플레이어한테 _trapDamage, _knockbackForce _invincibleTime
    }
}
