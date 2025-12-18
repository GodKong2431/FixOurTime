using UnityEngine;

public class ColdDebuff : IDebuff<Player>
{
    public string Name => "추움";

    public float Duration { get; set; }
    float _originalSpeed;

    public ColdDebuff(float duration)
    {
        Duration = duration;
    }

    public void OnEnter(Player conText)
    {
        _originalSpeed = conText.MoveSpeed;
        conText.MoveSpeed *= 0.7f; // 30% 감소
        conText.Spr.color = Color.cyan;
    }

    public void OnExecute(Player conText)
    {
        
    }

    public void OnExit(Player conText)
    {
        conText.MoveSpeed = _originalSpeed; // 속도 복구
        conText.Spr.color = Color.white;
    }
}
