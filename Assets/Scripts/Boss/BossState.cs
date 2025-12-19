using UnityEngine;
using System.Collections;

public abstract class BossState
{
    protected BossBase _baseBoss;
    public BossState(BossBase boss)
    {
        _baseBoss = boss;
    }
    public abstract void Enter();
    public abstract IEnumerator Execute(); // 코루틴으로 실행 대기
    public abstract void Exit();
}