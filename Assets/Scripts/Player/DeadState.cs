using UnityEngine;

public class DeadState : IPlayerState
{
    Player _player;
    float _timer = 2.0f;
    public DeadState(Player player)
    {
        _player = player;
    }
    public void Enter()
    {
        //사망 애니메이션 여기 넣기
        _player.Rb.linearVelocity = Vector2.zero;
        _player.Rb.bodyType = RigidbodyType2D.Kinematic;
        Debug.Log("사망");
    }

    public void Exit()
    {
        _player.Rb.bodyType = RigidbodyType2D.Dynamic;
        _timer = 2.0f;
    }

    public void Update()
    {
        _timer -= Time.deltaTime;
        if(_timer <= 0)
        {
            _player.Respawn();
        }
    }
}
