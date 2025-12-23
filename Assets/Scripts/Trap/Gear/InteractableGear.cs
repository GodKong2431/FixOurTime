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
    bool _isFinal = false; //마지막 인덱스인지 확인용

    private GearStateType _currentState;

    float rotateSpeed;

    protected void Awake()
    {
        _move = GetComponent<PointMoveObj>();
        _currentState = GearStateType.GearIdleState;

        rotateSpeed = _rotateSpeed;
        _rotateSpeed = 0;
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
            _rotateSpeed = rotateSpeed;
            //기존 체인지 포인트는 인덱스가 _currentIndex += _wayDir 때문에 범위벗어날라해서 여기서 직접 거리체크
            if (Vector3.Distance(transform.position,_move.NextPoint.position)<0.05f)
            {
                transform.position = _move.NextPoint.position;

                if (_move.CurrentIndex == _move.Points.Length - 1)
                {
                    Debug.Log("마지막 인덱스 도달 상호작용x");
                    _rotateSpeed = 0;
                    _isFinal = true; // 잠금 상태 활성화
                }

                if (_onOffObjs != null)
                    ObjOnOff();

                _currentState = GearStateType.GearIdleState;
            }
        }
    }
    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        if (_currentState == GearStateType.GearMoveState || _isFinal) return;
        bool dir = Mathf.Sign(hitPos.x - transform.position.x) == Mathf.Sign(_move.Points[0].transform.position.x - _move.Points[1].transform.position.x) ? false : true;
        HitGear(dir);
    }
    public void HitGear(bool direction)
    {
        if(_isFinal || direction) return;
        //다음이동 포인트 계산용
        int targetIndex = _move.CurrentIndex + 1;
        //인덱스가 범위 초과 하는지 체크
        if(targetIndex >= 0 && targetIndex < _move.Points.Length)
        {
            _move.CurrentIndex = targetIndex;
            _move.NextPoint = _move.Points[targetIndex];

            _currentState = GearStateType.GearMoveState;
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
