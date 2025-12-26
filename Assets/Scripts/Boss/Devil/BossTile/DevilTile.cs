using System.Collections;
using UnityEngine;

public class DevilTile : MonoBehaviour
{
    enum TileDir
    {
        Left,
        Right
    }



    [Header("움질일 거리")]
    [SerializeField] private float _distance = 37;
    [SerializeField] private float _duration = 2;
    [SerializeField] private TileDir _dir = TileDir.Left;

    Vector2 _startPos;
    Vector2 _moveDir;
    Vector2 _endPos;

    Coroutine _mouveCoroutine;

    private void Awake()
    {
        _startPos = transform.position;
        _moveDir = _dir == TileDir.Left ? Vector2.left : Vector2.right;

        _endPos = _startPos + _moveDir * _distance;
    }

    public void Move()
    {
        if (_mouveCoroutine != null)
            StopCoroutine(_mouveCoroutine);

        _mouveCoroutine = StartCoroutine(MoveCoroutine(_startPos, _endPos));
    }

    private IEnumerator MoveCoroutine(Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            float t = elapsed / _duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}
