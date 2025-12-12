using UnityEngine;

public class Gear : TrapBase
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("플레이어 접촉");
        //플레이어한테 Transform, _trapDamage, _knockbackForce _invincibleTime
    }
}
