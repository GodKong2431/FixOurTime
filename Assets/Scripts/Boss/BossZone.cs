using UnityEngine;

public class BossZone : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("연결할 보스 스크립트")]
    [SerializeField] private BossBase _boss;

    [Tooltip("보스 발동에 필요한 대기 시간")]
    [SerializeField] private float _requiredTime = 2.0f;

    private float _stayTimer = 0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 1. 플레이어 태그 확인
        if (collision.CompareTag("Player"))
        {
            // 2. 머무르는 동안 시간 증가
            _stayTimer += Time.deltaTime;

            // 3. 일정 시간이 지나면 보스 활성화
            if (_stayTimer >= _requiredTime)
            {
                if (_boss != null)
                {
                    Debug.Log("보스존 진입 확인: 보스 등장!");
                    _boss.ActivateBoss();

                    // 보스가 켜졌으므로 이 트리거는 비활성화 
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 영역을 벗어나면 타이머 초기화
        if (collision.CompareTag("Player"))
        {
            _stayTimer = 0f;
        }
    }

    // 필요 시 외부에서 다시 켤 수 있도록 유지
    public void ResetTrigger()
    {
        _stayTimer = 0f;
        gameObject.SetActive(true);
    }
}