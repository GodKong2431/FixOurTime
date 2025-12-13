using UnityEngine;

public class MoveGear : Gear
{
    [SerializeField] protected Transform[] _points;
    [SerializeField] protected float _moveSpeed = 3f;

    protected Transform _nextPoint;
    protected int _currentIndex = 0;
    protected int _wayDir = 1;

    protected virtual void Awake()
    {
        transform.position = _points[0].position;
        _nextPoint = _points[_currentIndex];
    }


    protected void MoveNextPoint()
    {
        //내 위치 정해진 위치로 이동
        transform.position = Vector3.MoveTowards
                (
                    transform.position,
                    _nextPoint.position,
                    _moveSpeed * Time.deltaTime
                );
    }

    protected bool ChangeNextPoint()
    {
        //거리가 0.05f보다 가까워 지면 방향 인덱스 변경
        if (Vector3.Distance(transform.position, _nextPoint.position) < 0.05f)
        {
            //인덱스 더하기
            _currentIndex += _wayDir;

            //마지막 인덱스에 도착했거나 처음 인덱스에 도착했으면 방향 변경
            if (_currentIndex >= _points.Length - 1 || _currentIndex <= 0)
                _wayDir *= -1;

            //목표 포인트 변경
            _nextPoint = _points[_currentIndex];
            return true;
        }
        return false;
    }

    protected void OnDrawGizmos()
    {
        //포인트 없으면 넘기기
        if (_points == null || _points.Length == 0) return;

        //포인트 색깔
        Gizmos.color = Color.yellow;

        //포인트 그리기
        foreach (var p in _points)
            Gizmos.DrawSphere(p.position, 0.3f);

        //간선 색깔
        Gizmos.color = Color.cyan;

        //간선 그리기
        for (int i = 0; i < _points.Length - 1; i++)
            if (_points[i] != null && _points[i + 1] != null)
                Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
    }
}
