using UnityEngine;
using UnityEngine.Video;

public class StartSceneManager : MonoBehaviour
{
    private VideoPlayer _video;

    void Awake()
    {
        _video = GetComponent<VideoPlayer>();
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
