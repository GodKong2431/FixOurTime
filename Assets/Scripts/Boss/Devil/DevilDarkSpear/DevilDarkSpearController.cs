using System.Collections;
using UnityEngine;

public class DevilDarkSpearController : MonoBehaviour
{
    [Header("시간 제한")]
    [SerializeField] private float _stayTime;

    [Header("스킬")]
    [SerializeField] private DevilDarkSpear _devilDarkSpear;

    [Header("BoxCast 설정")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private Transform boxPos;
    [SerializeField] private Vector2 boxOffset;

    private float _currentStayTime = 0f;
    private bool _isAttacking = false;

    public Coroutine _checkCoroutine;

    private Boss3DevilData _data;

    public void Initialize(Stage3DevilBoss boss)
    {
        _data = boss.Data;
        if (_data != null)
        {
            _stayTime = _data.SpearStayTime;
            _devilDarkSpear.Configure(_data.SpearMoveSpeed, _data.SpearDamage);
        }
    }

    private void Awake()
    {
        var boss = GetComponentInParent<Stage3DevilBoss>();
        if (boss != null)
        {
            Initialize(boss);
        }
    }

    private void Start()
    {
        StartCoroutine(CheckStayTime());
    }

    public IEnumerator CheckStayTime()
    {
        _currentStayTime = 0f;

        while (true)
        {
            if (_isAttacking)
            {
                yield return null;
                continue;
            }

            RaycastHit2D hit = Physics2D.BoxCast(boxPos.position + (Vector3)boxOffset, boxSize, 0f, Vector2.zero, 0f, 1 << 10);

            if (hit.collider != null)
            {
                _currentStayTime += Time.deltaTime;

                if (_currentStayTime >= _stayTime)
                {
                    yield return StartCoroutine(CallDarkSpear());
                    _currentStayTime = 0f;
                }
            }
            else
            {
                _currentStayTime = 0f;
            }

            yield return null;
        }
    }

    public IEnumerator CallDarkSpear()
    {
        if (_isAttacking) yield break;
        _isAttacking = true;

        _devilDarkSpear.AttackSpear();

        float delay = (_data != null) ? _data.SpearReturnDelay : 3.0f;
        yield return new WaitForSeconds(delay);

        _devilDarkSpear.ReturnSpear();
        _isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (boxPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxPos.position + (Vector3)boxOffset, boxSize);
        }
    }
}