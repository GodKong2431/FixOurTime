using UnityEngine;
using System.Collections;
public class BossStatusManager : MonoBehaviour
{
    // 외부에서 접근은 가능하지만(get), 값 설정은 내부에서만(private set) 가능하도록 설정
    public static BossStatusManager Instance { get; private set; }

    [Header("Boss CSV Info")]
    [SerializeField] private int _bossId = 101;       // 골렘 ID
    [SerializeField] private float _bossMaxHp = 100;  // 최대 체력

    // 현재 체력 (수치 확인용)
    [SerializeField] private float _bossCurrentHp;

    [Header("Game State")]
    [SerializeField] private bool _isGameClear = false;

    // TODO: 나중에 데이터 매니저(DataManager)가 생기면 거기서 데이터를 받아오는 로직 추가
    // [SerializeField] private BossDataTableSO _bossDataTable; 

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 체력 초기화
        _bossCurrentHp = _bossMaxHp;
    }

    // 외부(투사체 등)에서 호출하여 데미지를 주는 메서드
    public void TakeDamage(float amount)
    {
        _bossCurrentHp -= amount;
        Debug.Log($"Boss HP: {_bossCurrentHp}");

        // 체력이 0 이하가 되면 클리어 처리
        if (_bossCurrentHp <= 0)
        {
            _bossCurrentHp = 0;
            OpenMapTop();
        }
    }

    // 맵 상단이 열리는 연출 (게임 클리어)
    private void OpenMapTop()
    {
        _isGameClear = true;
        Debug.Log("보스 처치를 처치했습니다. 맵이 열립니다.");

        // 맵 상단 오브젝트를 찾아서 비활성화
        //GameObject mapTop = GameObject.Find("MapTopObstacle");
        //if (mapTop != null)
        //{
        //    mapTop.SetActive(false);
        //}
    }
}