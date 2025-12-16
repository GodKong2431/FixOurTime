using System.Collections;
using UnityEngine;
enum GearStateType
{
    GearIdleState,
    GearMoveState
}

public class InteractableGear : MoveGear,IDamageable
{
    [SerializeField] private GameObject OnOffObj;
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
                OnOffObj.SetActive(!OnOffObj.activeSelf);
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
}
