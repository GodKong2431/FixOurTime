using UnityEngine;

public class MinuteHandBuff : IStatusEffect<Player>
{
    public string Name => "분침버프";

    public float Duration { get; set; }

    public bool IsPositive => true;
    float _originalMaxChargeTime;

    public MinuteHandBuff(float duration)
    {
        Duration = duration;
    }

    public void OnEnter(Player conText)
    {
        _originalMaxChargeTime = conText.MaxChargeTime;
        conText.MaxChargeTime *= 0.5f;
        Debug.Log("점프차지속도2배상승");
    }

    public void OnExecute(Player conText)
    {
        
    }

    public void OnExit(Player conText)
    {
        conText.MaxChargeTime = _originalMaxChargeTime;
    }
}
