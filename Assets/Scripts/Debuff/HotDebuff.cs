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
        Debug.Log("´õ¿ò»óÅÂ È¹µæ");
        conText.Spr.color = Color.orange;
    }

    public void OnExecute(Player conText)
    {

    }

    public void OnExit(Player conText)
    {
        conText.Spr.color = Color.white;
    }
}
