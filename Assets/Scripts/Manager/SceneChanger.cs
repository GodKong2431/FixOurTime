using System.Collections;
using UnityEngine;
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


    public bool _videoPlay = false; //영상 초기셋팅 꺼두기
    protected override void Awake()
    {
        base.Awake();

        if (_fadeCanvas == null)
            _fadeCanvas = GetComponentInChildren<CanvasGroup>(true);
        if (_fadeCanvas != null)
        {
            _fadeCanvas.alpha = 0f;
            _fadeCanvas.gameObject.SetActive(false);
        }
    }
    public void StartNewGame(string sceneName)
    {
        StartCoroutine(LoadStageCoroutine(sceneName, false, true));
        _videoCanvas.gameObject.SetActive(true);
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
        // 1. 페이드 아웃 (검은 화면으로)
        yield return StartCoroutine(Co_Fade(0f, 1f));

        // 2. 비디오 재생 (playVideo 매개변수에 의해 결정)
        if (playVideo && _newGamePlayer != null)
        {
            _newGamePlayer.time = 0;
            _newGamePlayer.Play();

            // 비디오가 실제로 재생 시작될 때까지 대기
            yield return new WaitUntil(() => _newGamePlayer.isPlaying);

            // 비디오가 보이게 페이드 알파만 0으로 (캔버스는 켜진 상태)
            _fadeCanvas.alpha = 0f;

            // 영상이 끝날 때까지 대기
            while (_newGamePlayer.isPlaying)
            {
                // 스킵 기능: 아무 키나 누르면 영상 정지
                //if (Input.anyKeyDown) _newGamePlayer.Stop();
                yield return null;
            }

            // 영상 종료 후 다시 페이드 알파 1 (씬 로딩 중 화면 가림)
            _fadeCanvas.alpha = 1f;
            _videoCanvas.gameObject.SetActive(false);
        }

        // 3. 비동기 씬 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        // 로드 후 1초 대기 (안정화 시간)
        yield return new WaitForSecondsRealtime(1f);

        // 4. 리커넥트 및 데이터 복구
        if (CinemachinCamManager.Instance != null)
            CinemachinCamManager.Instance.Reconnect();

        if (useSavePos)
        {
            GameData data = GameDataManager.Load();
            Player player = FindAnyObjectByType<Player>();
            if (player != null) player.LoadPlayerData(data);
        }

        // 5. 페이드 인 (화면 밝아짐)
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
