using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Judgment : DamageableTrapBase
{
    private LineRenderer _judgmentLine;

    [Header("라인 굵기")]
    [SerializeField] private float _startLine = 0.1f;
    [SerializeField] private float _endLine = 0.1f;

    [Header("랜덤 범위")]
    [SerializeField] private float _ramdomDistance = 40;

    [Header("불 오브젝트")]
    [SerializeField] private GameObject _fireObj;

    [Header("불 스폰 오프셋")]
    [SerializeField] private float _fireObjoffset = 1;

    private Coroutine _lineOffCoroutine;


    private void Awake()
    {
        _judgmentLine = GetComponent<LineRenderer>();

        _judgmentLine.startWidth = _startLine;
        _judgmentLine.endWidth = _endLine;
    }

    public void StartJudgment(Vector2 pos)
    {
        Vector2 stratpos = GetRandomPos();

        _judgmentLine.enabled = true;
        ConnectLine(new Vector2(pos.x, transform.position.y), pos);
        LineRay(new Vector2(pos.x, transform.position.y), pos);

        if (_lineOffCoroutine != null)
            StopCoroutine(_lineOffCoroutine);

        _lineOffCoroutine = StartCoroutine(DisableLineAfterTime(1f));
    }

    private IEnumerator DisableLineAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        _judgmentLine.enabled = false;
    }

    private Vector2 GetRandomPos()
    {
        return new Vector2(transform.position.x + Random.Range(-_ramdomDistance, _ramdomDistance), transform.position.y);
    }

    private void ConnectLine(Vector2 start, Vector2 end)
    {
        _judgmentLine.positionCount = 2;
        _judgmentLine.useWorldSpace = true;

        _judgmentLine.SetPosition(0, start);
        _judgmentLine.SetPosition(1, end);
    }

    private void LineRay(Vector2 start, Vector2 end)
    {

        Vector2 dir = (end - start).normalized;
        float dist = Vector2.Distance(start, end);
        Vector2 fixedEnd = start + dir * dist;

        RaycastHit2D firsthit = Physics2D.Raycast(start, dir, dist, 1 << 10);
        if(firsthit)
        {
            if (firsthit.transform.TryGetComponent(out Player player))
            {
                player.TakeDamage(_trapDamage, _knockbackForce, start);
            }
        }
        
        RaycastHit2D[] secondhits = Physics2D.RaycastAll(fixedEnd, dir, 10, (1 << 10) | (1 << 31) | (1 << 29));
        if (secondhits.Length > 0)
        {
            foreach(RaycastHit2D hit in secondhits)
            {
                if (hit.transform.TryGetComponent(out Player player))
                {
                    player.TakeDamage(_trapDamage, _knockbackForce, start);
                    continue;
                }
                if(hit.transform.CompareTag("Ground"))
                {
                    SpawnFire(hit.point, hit.transform);
                    continue;
                }
            }
        }
    }

    public void SpawnFire(Vector2 pos, Transform platform)
    {
        GameObject obj = Instantiate(_fireObj);

        obj.transform.SetParent(platform);
        obj.transform.position = pos + new Vector2(0, _fireObjoffset);
    }
}
