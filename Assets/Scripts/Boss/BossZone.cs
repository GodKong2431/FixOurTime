using UnityEngine;

public class BossZone : MonoBehaviour
{
    [Header("보스 컨트롤러")]
    [SerializeField] private BossBase _boss;


    private float _requiredTime = 2.0f;

    private float _stayTimer = 0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 1. 태그 확인
        if (!collision.CompareTag("Player")) return;

        // 2. Player 컴포넌트 확인
        if (!collision.TryGetComponent(out Player player)) return;


        // 4. 영역 완전 진입 판정
        Bounds triggerBounds = GetComponent<Collider2D>().bounds;
        Bounds playerBounds = collision.bounds;

        bool xInside = playerBounds.min.x >= triggerBounds.min.x &&
                       playerBounds.max.x <= triggerBounds.max.x;

 
        bool yInside = playerBounds.max.y <= triggerBounds.max.y + 2.0f && // 점프 고려 여유값 추가
                       playerBounds.min.y >= triggerBounds.min.y - 0.5f;

        if (xInside && yInside)
        {
            _stayTimer += Time.deltaTime;

            if (_stayTimer >= _requiredTime)
            {
                if (_boss != null)
                {
                    Debug.Log("보스존 조건 충족: 보스 등장");
                    _boss.ActivateBoss();
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            _stayTimer = 0f; // 범위 밖으로 나가면 리셋
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _stayTimer = 0f;
        }
    }

    public void ResetTrigger()
    {
        Debug.Log("보스존 트리거 재활성화");
        _stayTimer = 0f;
        gameObject.SetActive(true);
    }
}