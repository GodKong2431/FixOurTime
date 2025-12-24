using System.Collections;
using UnityEngine;

public class DevilDarkSpear : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed = 2f;

    private Vector3 _startPos;
    private Vector3 _endPos;

    private Coroutine _moveCoroutine;

    private void Awake()
    {
        _startPos = transform.position;
        float height = GetHeight();
        _endPos = _startPos + Vector3.up * height;
    }
   
    private float GetHeight()
    {
        TryGetComponent<SpriteRenderer>(out var sr);
        return sr.bounds.size.y;
    }

    public void AttackSpear()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveTo(_endPos));
    }

    public void ReturnSpear()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveTo(_startPos));
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                _moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
}
