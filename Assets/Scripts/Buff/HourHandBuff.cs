using UnityEngine;

public class HourHandBuff : IStatusEffect<Player>
{
    public string Name => "시침버프";

    public float Duration { get; set; }

    public bool IsPositive => true;

    public HourHandBuff(float duration)
    {
        Duration = duration;
    }

    public void OnEnter(Player conText)
    {
        conText.IsInfiniteJumpEnabled = true;
        Debug.Log("무한점프 개방");
    }

    public void OnExecute(Player conText)
    {
        
    }

    public void OnExit(Player conText)
    {
        conText.IsInfiniteJumpEnabled = false;
        conText.IsInfiniteJumpLocked = false;
    }
}
