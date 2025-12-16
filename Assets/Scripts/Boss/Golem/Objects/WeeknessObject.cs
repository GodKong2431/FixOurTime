using UnityEngine;
using System.Collections;
public class WeaknessObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어의 공격에 맞았는지 확인
        if (other.CompareTag("PlayerAttack"))
        {
            Debug.Log("약점 타격 성공!");

            // 보스 체력 깎기
            if (BossStatusManager.Instance != null)
            {
                BossStatusManager.Instance.TakeDamage(10);
            }

            // 피격 후 즉시 소멸 (BossController가 다음 패턴으로 넘어가게 됨)
            Destroy(gameObject);
        }
    }
}