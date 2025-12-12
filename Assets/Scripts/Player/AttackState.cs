using UnityEngine;

public class AttackState : IPlayerState
{
    Player _player;
    public AttackState(Player player)
    {
        _player = player;
    }
    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
