using System.Collections;
using UnityEngine;


public class Laser : DamageableTrapBase, IDamageable
{
    [Header("쿨타임")]
    [SerializeField] private float _cooldown = 5f;
    [SerializeField] private float _warningTime = 1f;
    [SerializeField] private float _fireTime = 1f;

    [Header("레이 세팅")]
    [SerializeField] private float _maxDistance = 100f;
    [SerializeField] private Transform _firePoint;

    [Header("라인 렌더러")]
    [SerializeField] private LineRenderer _warningLine;
    [SerializeField] private LineRenderer _laserLine;

    private Vector2 EndPoint;
    private WaitForSeconds _cool;
    private WaitForSeconds _warningDur;
    private WaitForSeconds _fireDur;

    private BoxCollider2D box;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.enabled = false;
    }

    private void OnEnable()
    {
        _cool = new WaitForSeconds(_cooldown);
        _warningDur = new WaitForSeconds(_warningTime);
        _fireDur = new WaitForSeconds(_fireTime);
        GetDir();
        StartCoroutine(StartLaser());
    }

    private void GetDir()
    {
        Vector2 origin = _firePoint.position;

        RaycastHit2D rightHit = Physics2D.Raycast(origin, Vector2.right, _maxDistance, 1 << 30);
        RaycastHit2D leftHit = Physics2D.Raycast(origin, Vector2.left, _maxDistance, 1 << 30);

        float rightDist = rightHit ? rightHit.distance : _maxDistance;
        float leftDist = leftHit ? leftHit.distance : _maxDistance;

        if (rightDist >= leftDist)
            EndPoint = origin + Vector2.right * _maxDistance;
        else
            EndPoint = origin + Vector2.left * _maxDistance;

        Vector2 dir = EndPoint - (Vector2)_firePoint.position;
        float distance = dir.magnitude;
        dir.Normalize();

        box.size = new Vector2(distance, 0.2f);
        box.offset = Vector2.right * distance * 0.5f;

        transform.position = _firePoint.position;
        transform.right = dir;
    }

    private IEnumerator StartLaser()
    {
        while (true)
        {
            box.enabled = false;
            ShowLineRenderer(_warningLine, true);
            ShowLineRenderer(_laserLine, false);

            yield return _warningDur;

            box.enabled = true;
            ShowLineRenderer(_warningLine, false);
            ShowLineRenderer(_laserLine, true);

            yield return _fireDur;

            box.enabled = false;
            ShowLineRenderer(_laserLine, false);

            yield return _cool;
        }
    }

    private void ShowLineRenderer(LineRenderer line, bool active)
    {
        if (!line) return;

        line.enabled = active;
        if (active)
            UpdateLineRenderer(line);
    }

    private void UpdateLineRenderer(LineRenderer line)
    {
        line.positionCount = 2;
        line.SetPosition(0, _firePoint.position);
        line.SetPosition(1, EndPoint);
    }

    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        Destroy(gameObject);
    }
}