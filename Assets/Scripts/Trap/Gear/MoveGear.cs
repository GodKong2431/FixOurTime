using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEngine.GraphicsBuffer;

public class MoveGear : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float moveSpeed = 3f;

    Transform nextPoint;
    private int currentIndex = 0;
    private int _wayDir = 1;

    private void Awake()
    {
        transform.position = points[0].position;
        nextPoint = points[currentIndex];
    }

    void FixedUpdate()
    {
        //포인트 없으면 리턴
        if (points.Length == 0) return;
        
        //내 위치 정해진 위치로 이동
        transform.position = Vector3.MoveTowards
            (
                transform.position,
                nextPoint.position,
                moveSpeed * Time.fixedDeltaTime
            );

        //거리가 0.05f보다 가까워 지면 방향 인덱스 변경
        if (Vector3.Distance(transform.position, nextPoint.position) < 0.05f)
        {
            ChangeNextPoint();
        }
    }

    public void ChangeNextPoint()
    {
        //인덱스 더하기
        currentIndex += _wayDir;

        //마지막 인덱스에 도착했거나 처음 인덱스에 도착했으면 방향 변경
        if (currentIndex >= points.Length - 1 || currentIndex <= 0)
            _wayDir *= -1;

        //목표 포인트 변경
        nextPoint = points[currentIndex];
    }

    private void OnDrawGizmos()
    {
        //포인트 없으면 넘기기
        if (points == null || points.Length == 0) return;

        //포인트 색깔
        Gizmos.color = Color.yellow;

        //포인트 그리기
        foreach (var p in points)
        {
            Gizmos.DrawSphere(p.position, 0.3f);
        }

        //간선 색깔
        Gizmos.color = Color.cyan;

        //간선 그리기
        for (int i = 0; i < points.Length - 1; i++)
        {
            if (points[i] != null && points[i + 1] != null)
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }
    }
}
