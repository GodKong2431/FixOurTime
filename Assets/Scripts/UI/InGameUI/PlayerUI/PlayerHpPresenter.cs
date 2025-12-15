using UnityEngine;

public class PlayerHpPresenter
{
    private Player _player;
    private PlayerHpUI _plyerHpUI;

    public PlayerHpPresenter(Player player, PlayerHpUI hpUI)
    {
        _player = player;
        _plyerHpUI = hpUI;

        //_player.onHpChange += ChangeHp; //플레이어 이벤트 알림구독
    }

    void ChangeHp(float currentHp, float maxHp)
    {
        _plyerHpUI.ChangeHpBar(currentHp, maxHp);
    }

}
