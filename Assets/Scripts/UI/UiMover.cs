using UnityEngine;

public class UiMover : MonoBehaviour
{
    private RectTransform _rectPos;

    private void Awake()
    {
        _rectPos = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartCoroutine(UIUtill.UpMoving(_rectPos, 500f, 1f));
    }
}
