using UnityEngine;
using UnityEngine.Video;

public class StartSceneManager : MonoBehaviour
{
    private VideoPlayer _video;

    void Awake()
    {
        _video = GetComponent<VideoPlayer>();
    }
    private void Start()
    {
        //씬체인저가 존재하면서 비디오플레이어불값이 참이라면 (새게임시작으로옴)
        if (SceneChanger.Instance != null && SceneChanger.Instance._videoPlay == true)
        {
            gameObject.SetActive(true);
            _video.Play();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    void OnEnable()
    {
        _video.loopPointReached += OnVideoFinished;
    }

    void OnDisable()
    {
        _video.loopPointReached -= OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer source)    //비디오이벤트 핸들러 정의상 비디오매개변수 안써도 있어야함
    {
        //영상끝나면 비활성화
        gameObject.SetActive(false);
    }

    public void PlayIntroVideo()    //시작버튼용 함수
    {        
        gameObject.SetActive(true);
        _video.Play();
    }
}
