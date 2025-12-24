using UnityEngine;
using UnityEngine.Video;

public class IntroSceneManager : MonoBehaviour
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
        if (SceneChanger.Instance != null)
        {
            SceneChanger.Instance.ChangeScene("Title", false);   //타이틀씬이름
        }
    }
}
