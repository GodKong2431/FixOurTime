using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CinemachinCamManager : SingleTon<CinemachinCamManager>
{
    [Header("기본 설정")]
    [SerializeField] CinemachineCamera _activeCam;
    [SerializeField] Transform _player;
    [SerializeField] float _moveDuration = 0.8f;

    CinemachineBasicMultiChannelPerlin _noise;
    float _screenHeight;
    bool _isMoving;
    Camera _mainCam;

    protected override void Awake()
    {
        base.Awake();
        _mainCam = Camera.main;
        _noise = _activeCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        _screenHeight = _activeCam.Lens.OrthographicSize * 2f;
    }

    void LateUpdate()
    {
        
        if (_isMoving || _player == null) return;

        
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
        _noise.AmplitudeGain = intensity;
        _noise.FrequencyGain = frequency;
        yield return new WaitForSeconds(duration);
        _noise.AmplitudeGain = 0f;
        _noise.FrequencyGain = 0f;
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
}
