using UnityEngine;

public class SecondHandBuff : IStatusEffect<Player>
{
    public string Name => "초침버프";

    public float Duration { get; set; }

    public bool IsPositive => true;

    float _speedBuff;

    public SecondHandBuff(float duration)
    {
        Duration = duration;
    }

    public void OnEnter(Player conText)
    {
        _speedBuff = conText.MoveSpeed * 0.3f;
        conText.MoveSpeed += _speedBuff;
        Debug.Log("이속30퍼 증가");
    }

    public void OnExecute(Player conText)
    {
        
    }

    public void OnExit(Player conText)
    {
        conText.MoveSpeed -= _speedBuff;
    }
}
