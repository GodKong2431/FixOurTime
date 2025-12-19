using UnityEngine;

public class BossZone : MonoBehaviour
{
    [Header("보스 컨트롤러")]
    [SerializeField] private BossBase _boss;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 1. 태그 확인 (가드 클로즈)
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        // 2. Player 컴포넌트 가져오기
        if (!collision.TryGetComponent(out Player player))
        {
            return;
        }

        // 3.플레이어가 땅에 착지했는지 
        if (!player.IsGrounded)
        {
            return;
        }

        if (_boss != null)
        {
            _boss.ActivateBoss(); // 보스 깨우기

            // 더 이상 필요 없으니 트리거 끄기
            gameObject.SetActive(false);
        }
    }

    public void ResetTrigger()
    {
        Debug.Log("보스존 트리거 재활성화");
        gameObject.SetActive(true); // 다시 켜져서 플레이어를 감지할 준비
    }
}