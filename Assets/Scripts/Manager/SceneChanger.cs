using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneChanger : SingleTon<SceneChanger>
{
    [Header("페이드 설정")]
    [SerializeField] private CanvasGroup _fadeCanvas;
    [SerializeField] private float _fadeDuration = 0.5f;

    [Header("비디오 설정")]
    [SerializeField] private VideoPlayer _newGamePlayer;
    [SerializeField] Canvas _videoCanvas;

    private bool _isSkipTriggered = false;
    public bool _videoPlay = false; //영상 초기셋팅 꺼두기
    protected override void Awake()
    {
        base.Awake();

        if (_fadeCanvas != null)
        {
            _fadeCanvas.alpha = 0f;
            _fadeCanvas.gameObject.SetActive(false);
        }
    }
    public void StartNewGame(string sceneName)
    {
        GameDataManager.DeleteSaveData();

        if (_fadeCanvas != null) { _fadeCanvas.alpha = 1f; _fadeCanvas.gameObject.SetActive(true); }
        if (_videoCanvas != null) _videoCanvas.gameObject.SetActive(false);

        StartCoroutine(LoadStageCoroutine(sceneName, false, true));
    }
    public void LoadGameFromSave()
    {
        GameData data = GameDataManager.Load();
        bool hasData = !string.IsNullOrEmpty(data.sceneName);
        string targetScene = hasData ? data.sceneName : "stage1";

        // 데이터가 없으면 인트로를 재생(true), 있으면 바로 로드(false)
        StartCoroutine(LoadStageCoroutine(targetScene, hasData, !hasData));
    }

    public void ChangeScene(string sceneName, bool useSavePos = false, bool playVideo = false)
    {
        StartCoroutine(LoadStageCoroutine(sceneName, useSavePos, playVideo));
    }

    private IEnumerator LoadStageCoroutine(string sceneName, bool useSavePos, bool playVideo)
    {
        _fadeCanvas.alpha = 1f;
        _fadeCanvas.gameObject.SetActive(true);

        // 비디오 재생
        if (playVideo && _newGamePlayer != null)
        {
            _isSkipTriggered = false;
            var token = InputSystem.onAnyButtonPress.Call(ctrl => _isSkipTriggered = true);
            

            _newGamePlayer.Prepare();

            yield return new WaitUntil(() => _newGamePlayer.isPrepared);

            if (_videoCanvas != null) _videoCanvas.gameObject.SetActive(true);

            _newGamePlayer.Play();

            yield return new WaitForSecondsRealtime(0.1f);
            _fadeCanvas.alpha = 0f;

            while (_newGamePlayer.isPlaying)
            {
                if (_isSkipTriggered)
                {
                    Debug.Log("전역 인풋에 의해 영상 스킵");
                    _newGamePlayer.Stop();
                    break;
                }
                yield return null;
            }
            token.Dispose();

            _fadeCanvas.alpha = 1f; // 영상 끝난 후 다시 암전
            if (_videoCanvas != null) _videoCanvas.gameObject.SetActive(false);
        }

        //비동기 씬 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        // 로드 후 1초 대기
        yield return new WaitForSecondsRealtime(1f);

        // 리커넥트 및 데이터 복구
        if (CinemachinCamManager.Instance != null)
            CinemachinCamManager.Instance.Reconnect();

        if (useSavePos)
        {
            GameData data = GameDataManager.Load();
            Player player = FindAnyObjectByType<Player>();
            if (player != null) player.LoadPlayerData(data);
        }

        yield return new WaitForSecondsRealtime(2f);
        //페이드 인 
        yield return StartCoroutine(Co_Fade(1f, 0f));
    }
    private IEnumerator Co_Fade(float start, float end)
    {
        if (_fadeCanvas == null) yield break;

        _fadeCanvas.gameObject.SetActive(true);
        float timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            _fadeCanvas.alpha = Mathf.Lerp(start, end, timer / _fadeDuration);
            yield return null;
        }
        _fadeCanvas.alpha = end;

        // 화면이 다 밝아졌으면 캔버스 꺼주기 (클릭 방해 방지)
        if (end <= 0f) _fadeCanvas.gameObject.SetActive(false);
    }

    //버튼용
    public void ChangeSceneFromButton(string sceneName)
    {
        ChangeScene(sceneName, false);
    }
    public void ChangeSceneFromButtonWithSave(string sceneName)
    {
        ChangeScene(sceneName, true);
    }
}
