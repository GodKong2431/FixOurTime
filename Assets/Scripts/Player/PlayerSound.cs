using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip _attackSfx;


    public void AttackSound()
    {
        SoundManager.instance.PlaySFX(_attackSfx);
    }
}
