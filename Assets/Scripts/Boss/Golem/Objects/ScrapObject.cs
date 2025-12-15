using UnityEngine;
using System.Collections;
public class ScrapObject : MonoBehaviour
{
    [Header("Spec")]
    [SerializeField] private float _scrapSpeed = 5.0f; // 이동 속도

    private Vector3 _moveDirection;

    // 생성 시 호출되어 날아갈 방향 설정
    public void Initialize(Vector3 dir)
    {
        _moveDirection = dir;
        Destroy(gameObject, 5.0f); // 5초 뒤 자동 삭제 (맵 밖으로 나감)
    }

    private void Update()
    {
        // 설정된 방향으로 등속 이동
        transform.Translate(_moveDirection * _scrapSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 피격 (고철)");
            // TODO: 플레이어 데미지 처리 로직 호출
            Destroy(gameObject);
        }
    }
}