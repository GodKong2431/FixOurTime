using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Chunk Size")]
    public float _height = 10f; // 세로 길이 (C 단위)

    [Header("Points")]
    public Transform _topPoint;
    public Transform _bottomPoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_topPoint.position + Vector3.left * 5f, _topPoint.position + Vector3.right * 5f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_bottomPoint.position + Vector3.left * 5f, _bottomPoint.position + Vector3.right * 5f);
    }
}
