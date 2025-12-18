using UnityEngine;
using System.Collections;

public class ConcreteObject : MonoBehaviour
{
    private BoxCollider2D _col;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _moveDuration;

    private bool _isMoving = false; // 움직임 여부
    private bool _hasHit = false;   // 플레이어 피격여부
    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();

        _col.isTrigger = false;
    }

    public void Initialize(bool isHorizontal, Vector3 mapCenter, float moveDuration)
    {
        _moveDuration = moveDuration;
        _startPos = transform.position;
        _targetPos = transform.position;

        if (isHorizontal)
        {
            _targetPos.x = mapCenter.x;
        }
        else
        {
            _targetPos.y = mapCenter.y;
        }

        _hasHit = false;

        StartCoroutine(AppearRoutine());
    }

    private IEnumerator AppearRoutine()
    {
        _isMoving = true;


        float t = 0;
        while (t < _moveDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(_startPos, _targetPos, t / _moveDuration);
            yield return null;
        }
        transform.position = _targetPos;

        _isMoving = false;

        gameObject.layer = LayerMask.NameToLayer("Ground");

    }

    public void StartRetract()
    {
        StartCoroutine(RetractRoutine());
    }

    private IEnumerator RetractRoutine()
    {
        float t = 0;
        float returnTime = 1.0f;

        while (t < returnTime)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(_targetPos, _startPos, t / returnTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 움직이는 중, 피격하지 않은 상태일 경우
        if (_isMoving && !_hasHit)
        {
            // 1. 컴포넌트 찾기
            if (collision.gameObject.TryGetComponent(out IDamageable target))
            {
                // 2.매개변수 Collision2D에서 바로 GetContact를 호출
                Vector2 hitPoint = collision.GetContact(0).point;

                // 3. 데미지 주기
                target.TakeDamage(10, 5f, hitPoint);

                _hasHit = true;
            }
        }
    }
}