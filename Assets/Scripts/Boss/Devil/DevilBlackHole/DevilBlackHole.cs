using UnityEngine;
using System.Collections;

public class DevilBlackHole : MonoBehaviour
{
    [SerializeField] private string _playerLayerName = "Player"; 
    [SerializeField] private string _groundLayerName = "Ground";

    private float _minScale;
    private float _maxScale;
    private float _growTime;
    private float _shrinkTime;
    private float _pullSpeed;

    private bool _isActive;

    private Coroutine _scaleCoroutine;
    private CircleCollider2D _col;

    private void Awake()
    {
        _col = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("BlackHole Trigger Enter");
        if (other.CompareTag("Player"))
        {
            // 이름으로 번호를 찾아옴 (만약 없으면 -1 반환)
            int playerLayer = LayerMask.NameToLayer(_playerLayerName);
            int groundLayer = LayerMask.NameToLayer(_groundLayerName);

            // 레이어 충돌 끄기
            Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_isActive) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        Vector2 direction = ((Vector2)transform.position - rb.position).normalized;

        rb.linearVelocity = direction * _pullSpeed;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("BlackHole Trigger Exit");
        if (other.CompareTag("Player"))
        {
            int playerLayer = LayerMask.NameToLayer(_playerLayerName);
            int groundLayer = LayerMask.NameToLayer(_groundLayerName);

            // 레이어 충돌 다시 켜기
            Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
        }
    }

    public void Initialize(float minScale,float maxScale,float growTime,float shrinkTime,float pullSpeed)
    {
        _minScale = minScale;
        _maxScale = maxScale;
        _growTime = growTime;
        _shrinkTime = shrinkTime;
        _pullSpeed = pullSpeed;

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