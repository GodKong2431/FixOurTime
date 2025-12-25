using UnityEngine;

public class CeilingCollider : MonoBehaviour
{
    private Collider2D _targetCollider;

    private void Awake()
    {
        _targetCollider = GetComponent<Collider2D>();

    }

    /// <summary>
    /// 콜라이더를 비활성화하여 플레이어가 통과할 수 있게 ㅎ ㅏㅁ
    public void OpenCeiling()
    {
        // _targetCollider가 연결 안 된 경우를 대비해 로그 추가
        if (_targetCollider == null)
        {
            Debug.LogError("CeilingCollider: Target Collider is NULL!");
            return;
        }

        _targetCollider.enabled = false;
        Debug.Log("Ceiling collider disabled, player can pass through.");
    }
}