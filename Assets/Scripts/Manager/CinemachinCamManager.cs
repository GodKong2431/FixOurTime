using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachinCamManager : SingleTon<CinemachinCamManager>
{
    [Header("기본 설정")]
    [SerializeField] float _moveDuration = 0.8f;

    [SerializeField]CinemachineCamera _activeCam;
    [SerializeField]Transform _player;
    CinemachineBasicMultiChannelPerlin _noise;
    Camera _mainCam;

    float _screenHeight;
    bool _isMoving;

    protected override void Awake()
    {
        base.Awake();
        //Reconnect();
    }

    public void Reconnect()
    {
        StopAllCoroutines();
        _mainCam = Camera.main;
        _activeCam = FindAnyObjectByType<CinemachineCamera>();

        if(GameManager.Instance != null)
        {
            _player = GameManager.Instance.Player.transform;
        }

        if (_activeCam != null)
        {
            // 노이즈 컴포넌트 가져오기
            _noise = _activeCam.GetComponent<CinemachineBasicMultiChannelPerlin>();

            _screenHeight = _activeCam.Lens.OrthographicSize * 2f;

            // 플레이어 참조
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                _player = GameManager.Instance.Player.transform;
            }

            Debug.Log($"카메라 재연결완료 화면높이: {_screenHeight}");
        }
        else
        {
            // 씬에 카메라가 없는 경우 경고 출력 (디버깅용)
            Debug.LogWarning("Reconnect 실패.");
        }
    }
    void LateUpdate()
    {
        
        if (_isMoving || _player == null || _activeCam == null || _mainCam == null) return;

        
        Vector3 vp = _mainCam.WorldToViewportPoint(_player.position);

        if (vp.y > 1f) StartCoroutine(Co_MoveScreen(1));
        else if (vp.y < 0f) StartCoroutine(Co_MoveScreen(-1));
    }

    // 기존 신빈님 로직 화면단위 이동코루틴
    private IEnumerator Co_MoveScreen(int direction)
    {
        _isMoving = true;

        
        Vector3 startPos = _activeCam.transform.position;
        Vector3 targetPos = startPos + new Vector3(0, _screenHeight * direction, 0);

        float t = 0f;
        while (t < _moveDuration)
        {
            t += Time.deltaTime;

            if (_activeCam == null)
            {
                _isMoving = false;
                yield break;
            }

            _activeCam.transform.position = Vector3.Lerp(startPos, targetPos, t / _moveDuration);
            yield return null;
        }

        _activeCam.transform.position = targetPos;
        _isMoving = false;
    }

    /// <summary>
    /// 화면 흔들기
    /// </summary>
    /// <param name="intensity">강도</param>
    /// <param name="frequency">빈도</param>
    /// <param name="duration">시간</param>
    public void Shake(float intensity, float frequency, float duration)
    {
        if(_noise != null)
        {
            StartCoroutine(Co_Shake(intensity, frequency, duration));
        }
    }

    private IEnumerator Co_Shake(float intensity, float frequency, float duration)
    {
        if (_noise == null) yield break;

        _noise.AmplitudeGain = intensity;
        _noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        if (_noise != null)
        {
            _noise.AmplitudeGain = 0f;
            _noise.FrequencyGain = 0f;
        }
    }

    public void ZoomTarget(float targetSize, float duration)
    {
        StartCoroutine(Co_Zoom(targetSize, duration));
    }

    private IEnumerator Co_Zoom(float targetSize, float duration)
    {
        float startSize = _activeCam.Lens.OrthographicSize;
        float elapsed = 0f;
        while (elapsed < targetSize)
        {
            elapsed += Time.deltaTime;
            _activeCam.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            yield return null;
        }
        _activeCam.Lens.OrthographicSize = targetSize;
    }

    public Vector3 GetCamPos()
    {
        return _activeCam.transform.position;
    }
    public void LoadCamPos(Vector3 savedPos)
    {
        _activeCam.transform.position = savedPos;
    }
}
