using UnityEngine;
using System.Collections;

public class BossWeaknessState : BossState
{
    public BossWeaknessState(BossController controller) : base(controller) { }

    public override void Enter()
    {
        _controller.weaknessObject.SetActive(true);
        Debug.Log("약점 노출");
    }

    public override IEnumerator Execute()
    {

        float timer = 0;
        float startHp = _controller.CurrentHp;

        while (timer < _controller.Data.weaknessDuration)
        {
            timer += Time.deltaTime;

            if (_controller.CurrentHp < startHp)
            {
                Debug.Log("약점 공략 성공 -> 패턴 종료");
                break;
            }
            yield return null;
        }
    }

    public override void Exit()
    {
        _controller.weaknessObject.SetActive(false);
    }
}