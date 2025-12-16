using System.Collections;
using UnityEngine;
enum GearStateType
{
    GearIdleState,
    GearMoveState
}

public class InteractableGear : MoveGear,IDamageable
{
    [Header("꺼질 오브젝트")]
    [SerializeField] private GameObject[] _onOffObjs;
    private GearStateType _currentState;

    protected override void Awake()
    {
        base.Awake();
        _currentState = GearStateType.GearIdleState;
    }
    private void Update()
    {
        if (_currentState == GearStateType.GearMoveState)
        {
            MoveNextPoint();
            if (ChangeNextPoint())
            {
                if (_onOffObjs != null)
                    ObjOnOff();
                _currentState = GearStateType.GearIdleState;
            }
        }
    }
    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        HitGear();
    }
    public void HitGear()
    {
        _currentState = GearStateType.GearMoveState;
    }
    public void ObjOnOff()
    {
        foreach (var obj in _onOffObjs)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
