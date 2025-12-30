using System.Collections;
using UnityEngine;

public class ClockCore : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        if (GameManager.Instance.Player.HasAllClockHands)
        {
            StartCoroutine(ClearGame());
        }
        else
        {
            Debug.Log("아이템 부족");
        }
    }

    private IEnumerator ClearGame()
    {
        SoundManager.Instance.PlaySFX("SFX_Core_Activate");
        yield return new WaitForSeconds(10);
        SceneChanger.Instance.ChangeScene("EndingScene");
    }
}
