using System.Collections;
using UnityEngine;

public class DevilDarkSpear : DamageableTrapBase
{
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed = 2f;

    private Vector3 _startPos;
    private Vector3 _endPos;

    private Coroutine _moveCoroutine;

    private bool _spearActive;

    private void Awake()
    {
        _startPos = transform.position;
        float height = GetHeight();
        _endPos = _startPos + Vector3.up * height;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_spearActive) return;
        base.OnCollisionEnter2D(collision);
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (!_spearActive) return;
        base.OnCollisionEnter2D(collision);
    }

    private float GetHeight()
    {
        TryGetComponent<SpriteRenderer>(out var sr);
        return sr.bounds.size.y;
    }

    public void AttackSpear()
    {
        _spearActive = true;
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveTo(_endPos));
    }

    public void ReturnSpear()
    {
        _spearActive = false;
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
