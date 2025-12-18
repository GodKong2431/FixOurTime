using UnityEngine;
using System.Collections;

public class ConcreteObject : MonoBehaviour
{
    [Header("충돌 설정")]
    [SerializeField] private Collider2D _headCollider;

    private BoxCollider2D _col;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _moveDuration;
    private BossData _data; // 데이터를 저장해둠

    private bool _isMoving = false; // 움직임 여부
    private bool _hasHit = false;   // 플레이어 피격여부
    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();

        _col.isTrigger = false;
    }

    public void Initialize(bool isHorizontal, Vector3 mapCenter, BossData data)
    {
        _data = data;

        _moveDuration = data.concreteMoveTime;


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
        float returnTime = _data != null ? _data.concreteRetractDuration : 1.0f;

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
                // collision.otherCollider : 나(Concrete)의 콜라이더 중 부딪힌 
                bool isHeadHit = (collision.otherCollider == _headCollider);

                float dmg = isHeadHit ? _data.concreteDamage : _data.concreteSideDamage;
                float kb = _data.concreteKnockback;

                Debug.Log($"[충돌 발생] 맞은 부위: {(isHeadHit ? "<color=red>머리(Head)</color>" : "<color=yellow>몸통(Body)</color>")}, 데미지: {dmg}");

                target.TakeDamage(dmg, kb, collision.GetContact(0).point);
                _hasHit = true;
            }
        }
    }
}