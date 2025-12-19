using UnityEngine;

public class DeadState : IState<Player>
{

    float _timer = 2.0f;

    public void Enter(Player _player)
    {
        //사망 애니메이션 여기 넣기
        _player.Rb.linearVelocity = Vector2.zero;
        _player.Rb.bodyType = RigidbodyType2D.Kinematic;

        _player.Anim.SetInteger(_player.animState, 4);

        _player.InvokeDeadEvent();

        Debug.Log("사망");
    }

    public void Exit(Player _player)
    {
        _player.Rb.bodyType = RigidbodyType2D.Dynamic;
        _player.Anim.SetInteger(_player.animState, 0);
        _timer = 2.0f;
    }

    public void Execute(Player _player)
    {
        _timer -= Time.deltaTime;
        if(_timer <= 0)
        {
            _player.Respawn();
        }
    }
}
