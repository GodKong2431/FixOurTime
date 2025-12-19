using UnityEngine;

public class ClockCore : MonoBehaviour, IDamageable
{
    //이펙트 연결 시킬꺼 있으면 직렬화추가
    [Header("테스트용 참조")]
    [SerializeField] Player _player;
    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        if (_player.HasAllClockHands)
        {
            Debug.Log("게임 클리어");
        }
        else
        {
            Debug.Log("아이템 부족");
        }
    }
}
