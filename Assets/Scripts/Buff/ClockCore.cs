using UnityEngine;

public class ClockCore : MonoBehaviour, IDamageable
{
    Player _player;
    private void Awake()
    {
        _player = GameManager.Instance.Player;
    }

    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        if (_player.HasAllClockHands)
        {
            SceneChanger.Instance.ChangeScene("EndingScene");
        }
        else
        {
            Debug.Log("아이템 부족");
        }
    }
}
