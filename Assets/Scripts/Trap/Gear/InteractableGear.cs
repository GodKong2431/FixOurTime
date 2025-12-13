using System.Collections;
using UnityEngine;

enum GearStateType
{
    GearIdleState,
    GearMoveState
}

public class InteractableGear : MoveGear
{
    private GearStateType _currentState;

    private bool _isTrigger = false;

    protected override void Awake()
    {
        base.Awake();
        _currentState = GearStateType.GearIdleState;
    }

    private void Update()
    {
        if(_currentState == GearStateType.GearMoveState)
        {
            MoveNextPoint();

            if (ChangeNextPoint())
            {
                _isTrigger = !_isTrigger;
                _currentState = GearStateType.GearIdleState;
            }
        }
    }

    public void HitGear()
    {
        _currentState = GearStateType.GearMoveState;
    }
}
