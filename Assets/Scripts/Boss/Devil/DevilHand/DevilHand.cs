using System.Collections;
using UnityEngine;


public class DevilHand : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed = 18f;

    [Header("공격 속도")]
    [SerializeField] private float _attackSpeed = 18f;

    private Transform _boss;

    private Vector2 _startPos;
    private Vector2 _returnPos;

    private Coroutine _routine;
    private bool _hitGround;

    private void Awake()
    {
        _boss = transform.parent;
    }

    public void BeginPattern(Vector2 bossOffset)
    {
        _startPos = bossOffset;
        _hitGround = false;

        _returnPos = transform.position;

        if (_routine != null)
            StopCoroutine(_routine);

        transform.SetParent(null);
    }

    public void MoveToStartPos()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(MoveToStartPosCoroutine());
    }

    public void Attack(Vector2 direction)
    {
        _hitGround = false;

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(AttackCoroutine(direction.normalized));
    }

    public void SpiralAttack(Vector2 center, float offset, float duration)
    {
        Vector2 start = transform.position;
        Vector2 end = center;

        StartCoroutine(MoveCurveAutoOffset(start, end, offset, duration));
    }

    IEnumerator MoveCurveAutoOffset(Vector2 start,Vector2 end, float offset,float duration)
    {
        Vector2 center = (start + end) * 0.5f;

        float xDist = Mathf.Abs(start.x - center.x);
        float dir = Mathf.Sign(end.x - start.x);
        if (dir == 0) dir = 1f;

        Vector2 offsetPos = center + Vector2.right * dir * xDist * offset;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector2 a = Vector2.Lerp(start, offsetPos, t);
            Vector2 b = Vector2.Lerp(offsetPos, end, t);
            Vector2 pos = Vector2.Lerp(a, b, t);

            transform.position = pos;
            yield return null;
        }

        transform.position = end;
    }

    public void MoveToReturnPos()
    {
        _hitGround = false;

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(ReturnCoroutine());
    }

    private IEnumerator MoveToStartPosCoroutine()
    {
        while (true)
        {
            Vector2 target = (Vector2)_boss.position + _startPos;

            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                _moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, target) < 0.05f)
                break;

            yield return null;
        }
    }

    private IEnumerator AttackCoroutine(Vector2 dir)
    {
        while (!_hitGround)
        {
            transform.position += (Vector3)(dir * _attackSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator ReturnCoroutine()
    {
        while (Vector2.Distance(transform.position, _returnPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                _returnPos,
                _moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        
        transform.SetParent(_boss);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            _hitGround = true;
        }
    }
}
