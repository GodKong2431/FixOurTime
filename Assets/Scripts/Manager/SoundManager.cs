using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("배경음볼륨")]
    [SerializeField] public float _bgmVolume = 1.0f;
    [Header("효과음볼륨")]
    [SerializeField] public float _sfxVolume = 1.0f;

    [Header("메인씬 배경음")] 
    [SerializeField] private AudioClip _mainBgmClip;    //메인씬 배경음

  

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        //볼륨 초기화
        _bgmSource.volume = _bgmVolume;
        _sfxSource.volume = _sfxVolume;

        _bgmSource.clip = _mainBgmClip;
        _bgmSource.Play();
    }

    //설정창 볼륨조절 함수
    public void UpdateBgmVolume(float volume)
    {
        //볼륨값저장 (페이드효과에도 이값기준으로 사용해야함)
        _bgmVolume = volume;
        //볼륨값 적용
        _bgmSource.volume = volume;
    }
    public void UpdateSfxVolume(float volume)
    {
        _sfxVolume = volume;
        _sfxSource.volume = volume;
    }


    //효과음 재생
    public void PlaySFX(AudioClip clip)
    {
        //효과음 중복재생가능하게
        _sfxSource.PlayOneShot(clip, _sfxVolume);
    }

    //BGM 재생 (즉시 재생용)
    //아래 페이드용으로 쓸지 이걸 쓸지 아직미정
    public void PlayBGM(AudioClip clip)
    {
        _bgmSource.clip = clip;
        _bgmSource.volume = _bgmVolume;
        _bgmSource.Play();
    }


    //BGM페이드 효과용 재생 (코루틴)
    public void FadePlayBgm(AudioClip clip, float fadeTime = 1.0f)  //fadeTime동안 볼륨조절
    {
        StopAllCoroutines();    //진행중인 코루틴은 끄기
        StartCoroutine(FadeBgmCoroutine(clip, fadeTime));
    }

    private IEnumerator FadeBgmCoroutine(AudioClip clip, float time)
    {
        float startVolume = _bgmSource.volume;  //시작볼륨
        float fadeTime = 0; //0초시작

        //페이드 아웃
        while (fadeTime < time)
        {
            //프레임단위로 시간누적
            fadeTime += Time.unscaledDeltaTime;

            // 볼륨을 0으로 보간
            _bgmSource.volume = Mathf.Lerp(startVolume, 0, fadeTime / time);
            yield return null;
        }
        //기존 볼륨 0이되면 밑에 코드 시작

        _bgmSource.clip = clip; //호출한 BGM으로 변경
        _bgmSource.volume = 0; // 새 BGM은 볼륨 0부터 시작
        _bgmSource.Play();

        fadeTime = 0; // 시간 초기화

        //페이드 인 (새볼륨이니깐 아웃과 반대로 진행)
        while (fadeTime < time)
        {
            fadeTime += Time.unscaledDeltaTime;
            // 볼륨을 원래 설정된 기존 BGMVolume 으로 올림
            _bgmSource.volume = Mathf.Lerp(0, _bgmVolume, fadeTime / time);
            yield return null;
        }

        //새로운 볼륨값으로 변경
        _bgmSource.volume = _bgmVolume;
    }
}
