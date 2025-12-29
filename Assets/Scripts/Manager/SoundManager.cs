using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum SoundType { BGM, SFX }

public class SoundManager : SingleTon<SoundManager>
{
    #region 필드

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float _bgmVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float _sfxVolume = 1f;

    [Header("Default BGM")]
    [SerializeField] private AudioClip mainBgm;

    public float BGMVolume => _bgmVolume;
    public float SFXVolume => _sfxVolume;

    /// 사운드 테이블에서 로드한 AudioClip 캐싱할 딕셔너리
    private readonly Dictionary<string, AudioClip> soundCache = new();

    // SFX 중복 재생 방지용 (재생 종료 시간 기록)
    private readonly Dictionary<string, float> playingSfx = new();

    private Camera mainCam;

    #endregion

    #region 유니티 라이프 사이클

    protected override void Awake()
    {
        base.Awake();
        mainCam = Camera.main;
    }

    private void Start()
    {
        InitBgm();
        LoadSoundTable();
    }

    private void Update()
    {
        CleanupFinishedSfx();
    }

    #endregion

    #region 초기화

    /// <summary>
    /// 기본 BGM 설정 및 재생
    /// </summary>
    private void InitBgm()
    {
        bgmSource.loop = true;
        bgmSource.volume = _bgmVolume;
        bgmSource.clip = mainBgm;
        bgmSource.Play();
    }

    /// <summary>
    /// CSV 사운드 테이블 기반으로 Resources에서 AudioClip 로드
    /// </summary>
    private void LoadSoundTable()
    {
        var table = CSVDataManager.Instance.Get<SoundTableData>("SoundTable");

        foreach (var row in table.rows)
        {
            string path = GetSoundPath(row);
            AudioClip clip = Resources.Load<AudioClip>(path);

            if (clip == null)
            {
                Debug.LogError($"[SoundManager] 사운드 못찾음 : {path}");
                continue;
            }

            soundCache[row.name] = clip;
        }
    }

    private static string GetSoundPath(SoundTableData row)
    {
        return row.soundtype == SoundType.BGM ? $"Sound/BGM/{row.name}" : $"Sound/SFX/{row.name}";
    }

    #endregion

    #region 볼륨 컨트롤

    public void UpdateBgmVolume(float volume)
    {
        _bgmVolume = volume;
        bgmSource.volume = volume;
    }

    public void UpdateSfxVolume(float volume)
    {
        _sfxVolume = volume;
    }

    #endregion

    #region SFX

    /// <summary>
    /// SFX 재생
    /// - 중복 재생 방지
    /// - worldPos가 지정되면 카메라 안에 있을 때만 재생
    /// </summary>
    public void PlaySFX(string name, Vector3? worldPos = null)
    {
        // 위치가 주어졌고, 카메라 밖이면 재생하지 않음
        if (worldPos.HasValue && !IsInCameraView(worldPos.Value))
            return;

        if (!CanPlaySfx(name, out var clip))
            return;

        sfxSource.PlayOneShot(clip, _sfxVolume);
        RegisterSfx(name, clip.length);
    }

    private bool CanPlaySfx(string name, out AudioClip clip)
    {
        clip = null;

        if (!soundCache.TryGetValue(name, out clip))
            return false;

        return !playingSfx.ContainsKey(name);
    }

    private void RegisterSfx(string name, float duration)
    {
        playingSfx[name] = Time.time + duration;
    }

    private void CleanupFinishedSfx()
    {
        if (playingSfx.Count == 0)
            return;

        float now = Time.time;
        var removeList = ListPool<string>.Get();

        foreach (var sfx in playingSfx)
        {
            if (now >= sfx.Value)
                removeList.Add(sfx.Key);
        }

        foreach (var key in removeList)
            playingSfx.Remove(key);

        ListPool<string>.Release(removeList);
    }

    #endregion

    #region BGM

    public void FadePlayBgm(string name, float fadeTime = 1f)
    {
        if (!soundCache.TryGetValue(name, out var clip))
            return;

        StopAllCoroutines();
        StartCoroutine(FadeBgmRoutine(clip, fadeTime));
    }

    private IEnumerator FadeBgmRoutine(AudioClip clip, float time)
    {
        yield return FadeVolume(bgmSource, bgmSource.volume, 0, time);

        bgmSource.clip = clip;
        bgmSource.Play();

        yield return FadeVolume(bgmSource, 0, _bgmVolume, time);
    }

    private static IEnumerator FadeVolume(AudioSource source, float from, float to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        source.volume = to;
    }

    #endregion

    #region 포지션 체크

    /// <summary>
    /// 월드 좌표가 카메라 화면 안에 있는지 체크 (2D)
    /// </summary>
    private bool IsInCameraView(Vector3 worldPos)
    {
        if (mainCam == null)
            return false;

        Vector3 vp = mainCam.WorldToViewportPoint(worldPos);

        return vp.z > 0 &&
               vp.x is >= 0f and <= 1f &&
               vp.y is >= 0f and <= 1f;
    }

    #endregion
}