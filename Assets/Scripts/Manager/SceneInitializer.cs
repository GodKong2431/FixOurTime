using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private VideoPlayer introVideoPlayer;
    [SerializeField] private string nextSceneName = "Title";

    void Start()
    {
        if (introVideoPlayer != null)
        {
            // 영상 재생이 끝났을 때 실행될 이벤트 연결
            introVideoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            // 영상이 없다면 바로 타이틀로 이동
            MoveToTitle();
        }
    }

    // 영상이 끝날 때 호출되는 메서드
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("인트로 영상 종료, 타이틀로 이동합니다.");
        MoveToTitle();
    }

    void MoveToTitle()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // 스킵버튼
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            MoveToTitle();
        }
    }
}
