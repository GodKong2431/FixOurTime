using UnityEngine;
using System.Collections;

public class ScreenCameraController : MonoBehaviour
{
    [Header("플레이어 위치")]
    [SerializeField] private Transform player;

    [Header("카메라 전환 속도")]
    [SerializeField] float _duration = 0.8f;

    //화면 전체 높이
    private float screenHeight;

    private bool isMoving;
    void Awake()
    {
        screenHeight = Camera.main.orthographicSize * 2f;
    }

    void LateUpdate()
    {
        if (isMoving) return;
        Vector3 vp = Camera.main.WorldToViewportPoint(player.position);

        if (vp.y > 1f)
        {
            StartCoroutine(MoveToScreen(1));
        }
        else if (vp.y < 0f)
        {
            StartCoroutine(MoveToScreen(-1));
        }
    }

    IEnumerator MoveToScreen(int targetIndex)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 target = start;
        target.y = transform.position.y + (screenHeight * targetIndex);

        
        float t = 0f;

        while (t < _duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, t / _duration);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}
