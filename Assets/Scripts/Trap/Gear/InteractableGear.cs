using System.Collections;
using UnityEngine;
enum GearStateType
{
    GearIdleState,
    GearMoveState
}

public class InteractableGear : Gear,IDamageable
{
    [Header("꺼질 오브젝트")]
    [SerializeField] private GameObject[] _onOffObjs;

    PointMoveObj _move;

    private GearStateType _currentState;

    protected void Awake()
    {
        _move = GetComponent<PointMoveObj>();
        _currentState = GearStateType.GearIdleState;

        //기어 시작 위치 0번으로 고정
        if(_move.Points != null && _move.Points.Length > 0)
        {
            _move.CurrentIndex = 0;
            _move.NextPoint = _move.Points[0];
            transform.position = _move.Points[0].position;
        }
    }
    private void Update()
    {
        if (_currentState == GearStateType.GearMoveState)
        {
            _move.MoveNextPoint();
            //기존 체인지 포인트는 인덱스가 _currentIndex += _wayDir 때문에 범위벗어날라해서 여기서 직접 거리체크
            if (Vector3.Distance(transform.position,_move.NextPoint.position)<0.05f)
            {
                transform.position = _move.NextPoint.position;

                if (_onOffObjs != null)
                    ObjOnOff();
                _currentState = GearStateType.GearIdleState;
            }
        }
    }
    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        int dir = (hitPos.x < transform.position.x) ? 1 : -1;
        HitGear(dir);
    }
    public void HitGear(int direction)
    {
        if (_currentState == GearStateType.GearMoveState)
        {
            return;
        }
        //다음이동 포인트 계산용
        int targetIndex = _move.CurrentIndex + direction;
        //인덱스가 범위 초과 하는지 체크
        if(targetIndex >= 0 && targetIndex < _move.Points.Length)
        {
            _move.CurrentIndex = targetIndex;
            _move.NextPoint = _move.Points[targetIndex];

            _currentState = GearStateType.GearMoveState;
        }
        else
        {
            Debug.Log("이쪽방향은 포인트없음");
        }
    }
    public void ObjOnOff()
    {
        foreach (var obj in _onOffObjs)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
