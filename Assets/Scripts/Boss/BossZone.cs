using UnityEngine;
using UnityEngine.Events;

public class BossZone : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("연결할 보스 스크립트")]
    [SerializeField] private BossBase _boss;

    [Tooltip("보스 발동에 필요한 대기 시간")]
    [SerializeField] private float _requiredTime = 2.0f;

    [Header("Events")]
    [Tooltip("보스존이 활성화될 때 실행될 이벤트")]
    public UnityEvent OnBossZoneActivated;

    private float _stayTimer = 0f;
    private bool _isPlayerInside = false; // 플레이어가 안에 있는지 여부

    private void OnEnable()
    {
        _stayTimer = 0f;
        _isPlayerInside = false; // 켜질 때는 무조건 없는 걸로 간주
    }

    private void Update()
    {
        // 물리 엔진 상태와 상관없이 매 프레임 검사
        if (_isPlayerInside)
        {
            _stayTimer += Time.deltaTime;

            if (_stayTimer >= _requiredTime)
            {
                if (_boss != null)
                {
                    Debug.Log("보스존 진입 확인: 보스 등장!");


                    if (!_boss.gameObject.activeSelf)
                    {
                        _boss.gameObject.SetActive(true);
                    }
                    _boss.ActivateBoss();

                    OnBossZoneActivated?.Invoke();

                    gameObject.SetActive(false); // 보스 등장 후 트리거 비활성화
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInside = true; // 들어왔음 체크
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
        _isPlayerInside = false;
        gameObject.SetActive(true);
    }
}