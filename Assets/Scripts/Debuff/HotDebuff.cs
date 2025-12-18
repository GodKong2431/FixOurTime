using UnityEngine;

public class HotDebuff : IDebuff<Player>
{
    public string Name => "´õ¿ò";
    public float Duration { get; set; }

    public HotDebuff(float duration)
    {
        Duration = duration;
    }

    public void OnEnter(Player conText)
    {

    }

    public void OnExecute(Player conText)
    {

    }

    public void OnExit(Player conText)
    {

    }
}
