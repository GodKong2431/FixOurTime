using System.Collections;
using UnityEngine;

public class AutoMoveGear : Gear
{
    PointMoveObj _move;

    private void Awake()
    {
        _move = GetComponent<PointMoveObj>();
    }

    private void Start()
    {
        StartCoroutine(AutoMove());
    }

    IEnumerator AutoMove()
    {
        while (true)
        {
            _move.MoveNextPoint();
            if(_move.ChangeNextPoint())
            {
                _rotateSpeed = -_rotateSpeed;
            }

            yield return null;
        }
    }
}
