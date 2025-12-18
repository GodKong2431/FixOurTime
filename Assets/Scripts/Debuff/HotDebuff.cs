using UnityEngine;

public class HotDebuff : IDebuff<Player>
{
    public string Name => "더움";

    public float Duration { get; set; }

    public HotDebuff(float duration)
    {
        Duration = duration;
    }

    public void OnEnter(Player conText)
    {
        Debug.Log($"{Name} 디버프 발생!");
        conText.Spr.color = Color.orange;
    }

    public void OnExecute(Player conText)
    {
        
    }

    public void OnExit(Player conText)
    {
        Debug.Log($"{Name} 디버프 종료");
        conText.Spr.color = Color.white;
    }
}
