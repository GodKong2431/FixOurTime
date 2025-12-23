using UnityEngine;

public class BossZone : MonoBehaviour
{
    [Header("보스 컨트롤러")]
    [SerializeField] private BossBase _boss;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 1. 태그 확인
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

        // 4. 영역 완전 진입 판정 (X, Y축 좌표 비교)
        // Bounds.Contains 대신 직접 좌표를 비교하여 Z축 이슈 방지
        Bounds triggerBounds = GetComponent<Collider2D>().bounds;
        Bounds playerBounds = collision.bounds;

        // X축: 플레이어의 좌우가 트리거 범위 내에 있는지
        bool xInside = playerBounds.min.x >= triggerBounds.min.x &&
                       playerBounds.max.x <= triggerBounds.max.x;

        // Y축: 플레이어 머리와 발이 트리거 높이 범위 내에 있는지
        // (발바닥은 바닥 콜라이더와 겹칠 수 있으므로 -0.5f 정도 여유)
        bool yInside = playerBounds.max.y <= triggerBounds.max.y &&
                       playerBounds.min.y >= triggerBounds.min.y - 0.5f;

        if (xInside && yInside)
        {
            if (_boss != null)
            {
                Debug.Log("보스존 트리거 작동: 보스가 등장합니다.");
                _boss.ActivateBoss(); // 보스 깨우기

                // 더 이상 필요 없으니 트리거 끄기
                gameObject.SetActive(false);
            }
        }
    }

    public void ResetTrigger()
    {
        Debug.Log("보스존 트리거 재활성화");
        gameObject.SetActive(true); // 다시 켜져서 플레이어를 감지할 준비
    }
}