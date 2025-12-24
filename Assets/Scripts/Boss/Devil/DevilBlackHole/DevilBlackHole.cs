using UnityEngine;
using System.Collections;

public class DevilBlackHole : MonoBehaviour
{
    private float _minScale;
    private float _maxScale;
    private float _growTime;
    private float _shrinkTime;
    private float _minPullForce;
    private float _maxPullForce;

    private bool _isActive;

    private Coroutine _scaleCoroutine;
    private CircleCollider2D _col;

    private void Awake()
    {
        _col = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_isActive) return;

        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb == null) return;

            Vector2 center = transform.position;
            Vector2 playerPos = rb.position;

            Vector2 direction = center - playerPos;
            float distance = direction.magnitude;

            float radius = _col.radius * transform.localScale.x;

            float normalizedDistance = Mathf.Clamp01(distance / radius);

            float currentForce = Mathf.Lerp(_maxPullForce, _minPullForce, normalizedDistance);

            rb.AddForce(direction.normalized * currentForce * Time.deltaTime, ForceMode2D.Impulse);
        }
    }

    public void Initialize(float minScale,float maxScale,float growTime,float shrinkTime,float minPullForce, float maxPullForce)
    {
        _minScale = minScale;
        _maxScale = maxScale;
        _growTime = growTime;
        _shrinkTime = shrinkTime;
        _minPullForce = minPullForce;
        _maxPullForce = maxPullForce;

        transform.localScale = Vector3.one * minScale;
    }

    public void Activate()
    {
        _isActive = true;
        _scaleCoroutine = StartCoroutine(GrowCoroutine());
    }

    public void Deactivate()
    {
        _isActive = false;

        if (_scaleCoroutine != null)
            StopCoroutine(_scaleCoroutine);

        _scaleCoroutine = StartCoroutine(ShrinkCoroutine());
    }

    private IEnumerator GrowCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < _growTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _growTime;
            transform.localScale = Vector3.one * Mathf.Lerp(_minScale, _maxScale, t);
            yield return null;
        }

        transform.localScale = Vector3.one * _maxScale;
    }

    private IEnumerator ShrinkCoroutine()
    {
        float elapsed = 0f;
        float startScale = transform.localScale.x;

        while (elapsed < _shrinkTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _shrinkTime;
            float scale = Mathf.Lerp(startScale, 0f, t);
            transform.localScale = Vector3.one * scale;
            yield return null;
        }

        Destroy(gameObject);
    }
}