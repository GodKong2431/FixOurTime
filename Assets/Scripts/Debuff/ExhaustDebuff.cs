using UnityEngine;

public class ExhaustDebuff : IStatusEffect<Player>
{
    public string Name => "Å»Áø";

    public float Duration { get; set; }
    public bool IsPositive => false;

    float _timer = 0f;
    float _damageCooldown = 1.0f;
    float _damage = 5.0f;

    public ExhaustDebuff(float duration)
    {
        Duration = duration;
    }

    public void OnEnter(Player conText)
    {
        Debug.Log("Å»Áø°É¸²");
        conText.Spr.color = Color.red;
    }

    public void OnExecute(Player conText)
    {
        _timer += Time.deltaTime;

        if(_timer >= _damageCooldown)
        {
            conText.TakeDamage(_damage, 0, conText.transform.position);
            _timer = 0f;
        }
    }

    public void OnExit(Player conText)
    {
        Debug.Log("Å»Áø »óÅÂ ÇØÁ¦");
        conText.Spr.color = Color.white;
    }
}
