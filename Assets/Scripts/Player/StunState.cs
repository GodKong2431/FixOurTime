using UnityEngine;

public class StunState : IPlayerState
{
    Player _player;
    float _timer;
    public StunState(Player player)
    {
        _player = player;
    }
    public void Enter()
    {
        _timer = 0f;
        _player.Rb.linearVelocity = Vector2.zero;

        //스턴 애니메이션 재생
        _player.Spr.color = Color.gray;
        Debug.Log("스턴");
    }

    public void Exit()
    {
        _player.Rb.linearVelocity = Vector2.zero;
        _player.Spr.color = Color.white;
    }

    public void Update()
    {
        _timer += Time.deltaTime;

        if(_timer >= _player.StunDuration)
        {
            _player.SetState(new IdleState(_player));
        }

    }
}
