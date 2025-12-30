using UnityEngine;
using System.Collections;

public class BossWeaknessState : BossState
{
    public BossWeaknessState(Stage1Boss _boss) : base(_boss) { }

    public override void Enter()
    {
        Stage1Boss boss = _baseBoss as Stage1Boss;
        if (boss != null && boss.WeaknessObject != null)
        {
            var weaknessScript = boss.WeaknessObject.GetComponent<WeaknessObject>();
            if (weaknessScript != null)
            {
                weaknessScript.SetExposedState();
            }
        }
    }

    public override IEnumerator Execute()
    {
        Stage1Boss _boss = _baseBoss as Stage1Boss;

        float timer = 0;
        float startHp = _boss.CurrentHp;

        float waitTime = 2.5f;

        while (timer < _boss.BossData.WeaknessDuration)
        {
            timer += Time.deltaTime;

            if (_boss.CurrentHp < startHp)
            {
                Debug.Log("약점 공략 성공 -> 패턴 종료");

                yield return new WaitForSeconds(waitTime);
                break;
            }

            yield return null;
        }
    }

    public override void Exit()
    {

        Stage1Boss _boss = _baseBoss as Stage1Boss;
        if (_boss != null && _boss.WeaknessObject != null)
        {
            var weaknessScript = _boss.WeaknessObject.GetComponent<WeaknessObject>();
            if (weaknessScript != null)
            {
                weaknessScript.SetIdleState();
            }
        }
    }
}