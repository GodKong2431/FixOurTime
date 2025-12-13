using System.Collections;
using UnityEngine;

public class InteractableGear : MoveGear,IDamageable
{
    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        HitGear();
    }
    public void HitGear()
    {
        StartCoroutine(StartMove());
        
        //인덱스 더하기
        _currentIndex += _wayDir;

        //마지막 인덱스에 도착했거나 처음 인덱스에 도착했으면 방향 변경
        if (_currentIndex >= _points.Length - 1 || _currentIndex <= 0)
            _wayDir *= -1;

        //목표 포인트 변경
        _nextPoint = _points[_currentIndex];
    }

    

    IEnumerator StartMove()
    {
        MoveNextPoint();

        if(ChangeNextPoint())
            yield break;

        yield return null;
    }
}
