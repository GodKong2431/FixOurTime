using UnityEngine;

public class StunState : IState<Player>
{
    float _timer;

    public void Enter(Player _player)
    {
        _player.SetPhysicsMaterial(false);

        _timer = 0f;
        _player.Rb.linearVelocity = Vector2.zero;

        //스턴 애니메이션 재생
        _player.Spr.color = Color.gray;
        Debug.Log("스턴");
    }

    public void Exit(Player _player)
    {
        _player.Rb.linearVelocity = Vector2.zero;
        _player.Spr.color = Color.white;
    }

    public void Execute(Player _player)
    {
        _timer += _player.PlayerDeltaTime;

        if(_timer >= _player.StunDuration)
        {
            _player.SetState(new IdleState());
        }

    }
}
