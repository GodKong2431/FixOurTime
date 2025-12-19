using UnityEngine;

public class NightZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            // 낮 디버프 즉시 제거
            player.RemoveEffectByName("더움");
            player.RemoveEffectByName("탈진");

            //추움디버프 나가도 5초 유지
            player.AddEffect(new ColdDebuff(5f));
        }
    }
}
