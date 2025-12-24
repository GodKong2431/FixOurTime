using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EndingSceneManager : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] public float _scrollSpeed = 50f;  // 크레딧 올라가는 속도
    [SerializeField] public GameObject _creditUI;      // 크레딧 부모 오브젝트
    [SerializeField] public RectTransform _creditText; // 실제로 위로 올라갈 텍스트 (RectTransform)

    private VideoPlayer _video;
    private bool _isCreditStarting = false; // 텍스트 움직임 조건용

    void Awake()
    {
        _video = GetComponent<VideoPlayer>();
        _creditUI.SetActive(false); //시작할땐 UI 꺼두기
    }
    void OnEnable()
    {
        //영상이 끝날때 일어날 함수구독
        _video.loopPointReached += OnVideoFinished;
    }

    void OnDisable()
    {
        //구독해제
        _video.loopPointReached -= OnVideoFinished;
    }
    private void Update()
    {
        if (_isCreditStarting == true)
        {
            _creditText.anchoredPosition += Vector2.up * _scrollSpeed * Time.deltaTime;

            //크레딧 어느정도 올라가면 처음씬 호출
            if (_creditText.anchoredPosition.y > 2000f)
            {
                _isCreditStarting = false;
                SceneChanger.Instance.ChangeScene("Title", false);
            }
        }
    }

    void OnVideoFinished(VideoPlayer source)    //비디오이벤트 핸들러 정의상 비디오매개변수 안써도 있어야함
    {
        //영상 끝나면 실행할 로직
        _creditUI.SetActive(true);   // UI 활성화
        _isCreditStarting = true;   // 텍스트 움직일준비완료
    }
}
