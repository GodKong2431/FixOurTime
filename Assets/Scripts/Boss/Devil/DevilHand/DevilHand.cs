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

    private Coroutine _moveCoroutine;
    private bool _hitGround;

    private void Awake()
    {
        _boss = transform.parent;
        _returnPos = transform.position;
    }

    /// <summary>
    /// 시작 포지션을 설정하고 다른 코루틴이 실행중이라면 종료
    /// 부모에서 떨어짐
    /// </summary>s
    /// <param name="stratPos">시작 위치</param>
    public void BeginPattern(Vector2 stratPos)
    {
        _startPos = stratPos;
        _hitGround = false;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        transform.SetParent(null);
    }

    /// <summary>
    /// 실행중인 코루틴이 있다면 종료하고 시작지점으로 가게하는 코루틴
    /// </summary>
    public void MoveToStartPos()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveToStartPosCoroutine());
    }
    /// <summary>
    /// 시작지점으로 가는 코루틴
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 기존 코루틴을 제거하고 받아온 방향벡터로 공격 코루틴 실행
    /// </summary>
    /// <param name="direction">방향 벡터</param>
    public void Attack(Vector2 direction)
    {
        _hitGround = false;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(AttackCoroutine(direction.normalized));
    }
    /// <summary>
    /// 공격 코루틴 땅에 닿지 않았다면 계속 실행
    /// 받아온 방향값으로 이동
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private IEnumerator AttackCoroutine(Vector2 dir)
    {
        while (!_hitGround)
        {
            transform.position += (Vector3)(dir * _attackSpeed * Time.deltaTime);
            yield return null;
        }
    }
    /// <summary>
    /// 기존 실행중이던 코루틴 종료 시키고 시작점과 끝점을 체크해 베이어 곡선으로
    /// 스파이럴 공격 코루틴을 실행
    /// </summary>
    /// <param name="center"></param>
    /// <param name="offset"></param>
    /// <param name="duration"></param>
    public void SpiralAttack(Vector2 center, float offset, float duration)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        Vector2 start = transform.position;
        Vector2 end = center;

        _moveCoroutine = StartCoroutine(SpiralAttackCoroutine(start, end, offset, duration));
    }
    /// <summary>
    /// 세가지 포인트로 Lerp를 이용해 이동해 곡선의 형태를 나타내게 이동
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="offset"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator SpiralAttackCoroutine(Vector2 start,Vector2 end, float offset,float duration)
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

    /// <summary>
    /// 기존 코루틴 종료하고 복귀 코루틴 실행
    /// </summary>
    public void MoveToReturnPos()
    {
        _hitGround = false;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(ReturnCoroutine());
    }

    /// <summary>
    /// 시작했던 위치로 돌아가는 코루틴
    /// 다시 보스의 자식이 됨
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 땅인지 체크하고 _hitGround 켜줌
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            _hitGround = true;
        }
    }
}
