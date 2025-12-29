using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CeilingDoor : MonoBehaviour
{
    [Header("대기 설정")]
    [SerializeField] float _startDelay = 3f;

    [Header("문 오브젝트")]
    [SerializeField] Transform _leftDoor;
    [SerializeField] Transform _rightDoor;

    [Header("이동 설정")]
    [SerializeField] float _openDistance = 2f; // 옆으로 열릴 거리
    [SerializeField] float _openDuration = 1.5f;

    [Header("진동 설정")]
    [SerializeField] float _shakeIntensity = 1.2f;
    [SerializeField] float _shakeFrequency = 2.0f;

    [Header("흔들고 싶은 특정 카메라")]
    [SerializeField] private CinemachineCamera _targetShakeCam;

    private Collider2D _doorCollider;
    private bool _isOpened = false;

    private void Awake()
    {
        _doorCollider = GetComponent<Collider2D>();
    }

    public void OpenCeiling()
    {
        if (_isOpened) return;
        StartCoroutine(Co_OpenSequence());
    }

    IEnumerator Co_OpenSequence()
    {
        _isOpened = true;

        yield return new WaitForSeconds(_startDelay);

        //카메라 흔들기
        if (CinemachinCamManager.Instance != null && _targetShakeCam != null)
        {
            CinemachinCamManager.Instance.ShakeTargetCamera(_targetShakeCam, _shakeIntensity, _shakeFrequency, _openDuration);
        }

        // 사운드 재생
        SoundManager.Instance.PlaySFX("SFX_Stage_ClearEventQuake");

        // 문 열기 연출 (좌우로 이동)
        Vector3 leftStart = _leftDoor.localPosition;
        Vector3 rightStart = _rightDoor.localPosition;
        Vector3 leftTarget = leftStart + Vector3.left * _openDistance;
        Vector3 rightTarget = rightStart + Vector3.right * _openDistance;

        float t = 0;
        while (t < _openDuration)
        {
            t += Time.deltaTime;
            float p = t / _openDuration;

            // 부드러운 이동을 위해 Lerp 사용
            _leftDoor.localPosition = Vector3.Lerp(leftStart, leftTarget, p);
            _rightDoor.localPosition = Vector3.Lerp(rightStart, rightTarget, p);
            yield return null;
        }

        //콜라이더 비활성화 (이제 통과 가능)
        if (_doorCollider != null) _doorCollider.enabled = false;

        Debug.Log("스테이지 이동용 문 열림");
    }
}