using UnityEngine;
using System.Collections;

public class ConcreteObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _concreteMoveDuration = 0.5f; // 이동 시간
    [SerializeField] private float _concreteStayDuration = 5.0f; // 유지 시간

    private Vector3 _initialPos;
    private Vector3 _targetPos;
    private bool _isHorizontal;

    // 생성 시 호출 (이동 방향 및 목표 설정)
    public void Initialize(bool horizontal)
    {
        _initialPos = transform.position;
        _isHorizontal = horizontal;

        // 목표 지점 설정: 가로형이면 Y축 유지하고 X는 0(중앙), 세로형이면 반대
        if (_isHorizontal)
        {
            _targetPos = new Vector3(0, transform.position.y, 0);
        }
        else
        {
            _targetPos = new Vector3(transform.position.x, 0, 0);
        }

        StartCoroutine(MoveProcess());
    }

    private IEnumerator MoveProcess()
    {
        // 1. 0.5초에 걸쳐 중앙으로 이동
        float elapsed = 0;

        while (elapsed < _concreteMoveDuration)
        {
            transform.position = Vector3.Lerp(_initialPos, _targetPos, elapsed / _concreteMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = _targetPos;

        // 2. 이동 완료 후: 플레이어가 밟을 수 있는 벽/발판으로 변경
        gameObject.layer = LayerMask.NameToLayer("Ground");
        GetComponent<Collider2D>().isTrigger = false;

        // 3. 5초간 대기 (스킬 지속 시간)
        yield return new WaitForSeconds(_concreteStayDuration);

        // 4. 복귀 시작: 다시 뚫고 지나갈 수 있게 변경
        GetComponent<Collider2D>().isTrigger = true;

        elapsed = 0;
        float returnDuration = 1.0f; // 1초에 걸쳐서 원래 위치로

        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(_targetPos, _initialPos, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 복귀 완료 후 소멸
        Destroy(gameObject);
    }

    // 이동 중(공격 판정일 때) 충돌 처리
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 완전히 벽이 되기 전(이동 중)에는 데미지를 줌
            if (gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                Debug.Log("플레이어 피격 (콘크리트 쿵)");
            }
        }
    }
}