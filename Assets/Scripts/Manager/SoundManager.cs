using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public enum SoundType { BGM, SFX }

public class SoundManager : SingleTon<SoundManager> 
{ 
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource; 
    [SerializeField] private AudioSource _sfxSource; 

    [Header("Volumes")]
    [SerializeField] private float _bgmVolume = 1.0f; 
    [SerializeField] private float _sfxVolume = 1.0f; 

    [Header("Main BGM")]
    [SerializeField] private AudioClip _mainBgmClip; 

    private TableSOBase<SoundTableData> _soundData; 

    private readonly Dictionary<string, AudioClip> _soundDict = new(); 

    private readonly Dictionary<string, float> _playingSfx = new(); 

    private Camera _mainCamera; 
    public float BGMVolume => _bgmVolume; 
    public float SFXVolume => _sfxVolume; 


    #region 유니티 생명주기 

    protected override void Awake() 
    { 
        base.Awake(); 
        RefreshCamera(); 
    } 
    private void Start() 
    { 
        _bgmSource.loop = true; 
        _bgmSource.volume = _bgmVolume; 
        _bgmSource.clip = _mainBgmClip; 
        _bgmSource.Play(); 
        InitSoundData(); 
    } 
    private void Update() 
    { 
        CleanupFinishedSFX(); 
        RefreshCamera(); 
    } 
    #endregion 

    #region 초기화 
    private void RefreshCamera() 
    { 
        if (_mainCamera == null) 
            _mainCamera = Camera.main; 
    } 
    private void InitSoundData() 
    { 
        _soundData = CSVDataManager.Instance.Get<SoundTableData>("SoundTable"); 

        foreach (var row in _soundData.rows) 
        { 
            string path = GetResourcePath(row); 
            AudioClip clip = Resources.Load<AudioClip>(path); 

            if (clip == null) 
            { 
                Debug.LogError($"[SoundManager] 사운드 없음 : {path}"); 
                continue; 
            } 

            _soundDict[row.name] = clip; 
        } 
    } 
    private string GetResourcePath(SoundTableData row) 
    {
        return row.soundtype == SoundType.BGM
            ? $"Sound/BGM/{row.name}"
            : $"Sound/SFX/{row.name}";
    } 
    #endregion 

    #region UI 볼륨
    public void UpdateBgmVolume(float volume) 
    { 
        _bgmVolume = volume; 
        _bgmSource.volume = volume; 
    } 
    public void UpdateSfxVolume(float volume) 
    { 
        _sfxVolume = volume; 
    } 
    #endregion 

    #region SFX 

    //여러번 중복으로 호출가능한 메서드
    public void PlaySFXOneShot(string clipName)
    {
        if (!_soundDict.TryGetValue(clipName, out var clip)) return;

        _sfxSource.PlayOneShot(clip, _sfxVolume);
    }

    //화면 밖에서 소리 나는 메서드
    public void PlaySFX(string clipName) 
    { 
        if (!_soundDict.TryGetValue(clipName, out var clip)) return; 
        if (IsSFXPlaying(clipName)) return; 

        _sfxSource.PlayOneShot(clip, _sfxVolume); 
        RegisterSfx(clipName, clip.length); 
    } 

    // 화면 밖에서 사운드 안나게 하는 메서드
    public void PlaySFX(string clipName, Vector3 worldPos) 
    { 
        if (!_soundDict.TryGetValue(clipName, out var clip)) return; 
        if (!IsInCameraView2D(worldPos)) return; 
        if (IsSFXPlaying(clipName)) return; 

        _sfxSource.PlayOneShot(clip, _sfxVolume); 
        RegisterSfx(clipName, clip.length); 
    } 
    private bool IsSFXPlaying(string clipName) 
    { 
        return _playingSfx.ContainsKey(clipName); 
    } 
    private void RegisterSfx(string clipName, float duration) 
    { 
        _playingSfx[clipName] = Time.time + duration; 
    } 
    private void CleanupFinishedSFX() 
    { 
        if (_playingSfx.Count == 0) return; 

        float now = Time.time; 
        var removeList = ListPool<string>.Get(); 

        foreach (var kv in _playingSfx) 
        { 
            if (now >= kv.Value) removeList.Add(kv.Key); 
        } 

        foreach (var key in removeList) 
            _playingSfx.Remove(key); 

        ListPool<string>.Release(removeList); 
    } 
    #endregion 

    #region BGM 
    public void FadePlayBgm(string clipName, float fadeTime = 1f) 
    { 
        if (!_soundDict.TryGetValue(clipName, out var clip)) return; 

        StopAllCoroutines(); 
        StartCoroutine(FadeBgmCoroutine(clip, fadeTime)); 
    } 
    private IEnumerator FadeBgmCoroutine(AudioClip clip, float time) 
    { 
        float startVolume = _bgmSource.volume; 
        float t = 0f; 

        while (t < time) 
        { 
            t += Time.unscaledDeltaTime; 
            _bgmSource.volume = Mathf.Lerp(startVolume, 0, t / time); 
            yield return null; 
        } 

        _bgmSource.clip = clip; 
        _bgmSource.volume = 0; 
        _bgmSource.Play(); 
        t = 0f; 

        while (t < time) 
        { 
            t += Time.unscaledDeltaTime; 
            _bgmSource.volume = Mathf.Lerp(0, _bgmVolume, t / time); 
            yield return null; 
        } 

        _bgmSource.volume = _bgmVolume; 
    } 
    #endregion 

    #region 오브젝트 위치 체크 
    private bool IsInCameraView2D(Vector3 worldPos) 
    { 
        if (_mainCamera == null) return false; 

        Vector3 viewPos = _mainCamera.WorldToViewportPoint(worldPos); 

        return viewPos.z > 0 && viewPos.x >= 0f && viewPos.x <= 1f && viewPos.y >= 0f && viewPos.y <= 1f; 
    } 
    #endregion 
}