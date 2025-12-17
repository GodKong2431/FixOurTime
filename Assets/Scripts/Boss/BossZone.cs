using UnityEngine;

public class BossZone : MonoBehaviour
{
    [Header("보스 컨트롤러")]
    [SerializeField] private BossController _bossController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 들어왔는지 
        if (collision.CompareTag("Player"))
        {
            if (_bossController != null)
            {
                _bossController.ActivateBoss(); // 보스 깨우기

                // 더 이상 필요 없으니 트리거 끄기
                gameObject.SetActive(false);
            }
        }
    }
}