using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("BGM")]
    [SerializeField] private AudioSource _bgmSource;

    [Header("È¿°úÀ½")]
    [SerializeField] private AudioSource _sfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    //public void playSFX(AudioClip clip)
    //{
    //    PlayOneShot? play?
    //}
}
