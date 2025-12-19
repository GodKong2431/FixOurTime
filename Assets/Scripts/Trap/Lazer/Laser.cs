using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("쿨타임")]
    [SerializeField] private float _totalCooldown = 5f;
    [SerializeField] private float _warningTime = 1f;
    [SerializeField] private float _fireTime = 1f;

    [Header("레이 세팅")]
    [SerializeField] private float _maxDistance = 100f;
    [SerializeField] private Transform _firePoint;

    [Header("라인 렌더러")]
    [SerializeField] private LineRenderer _warningLine;
    [SerializeField] private LineRenderer _laserLine;

    private Vector2 fixedDirection;
    private Vector2 fixedEndPoint;

    private void OnEnable()
    {
        GetDir();
        StartCoroutine(LaserCycle());
    }

    private void GetDir()
    {
        Vector2 origin = _firePoint.position;

        RaycastHit2D rightHit = Physics2D.Raycast(origin, Vector2.right, _maxDistance, 1 << 30);
        RaycastHit2D leftHit = Physics2D.Raycast(origin, Vector2.left, _maxDistance, 1 << 30);

        float rightDist = rightHit ? rightHit.distance : _maxDistance;
        float leftDist = leftHit ? leftHit.distance : _maxDistance;


        if (rightDist >= leftDist)
        {
            fixedDirection = Vector2.right;
            fixedEndPoint = rightHit
                ? rightHit.point
                : origin + Vector2.right * _maxDistance;
        }
        else
        {
            fixedDirection = Vector2.left;
            fixedEndPoint = leftHit
                ? leftHit.point
                : origin + Vector2.left * _maxDistance;
        }
    }

    private IEnumerator LaserCycle()
    {
        while (true)
        {
            ShowLine(_warningLine, true);
            ShowLine(_laserLine, false);

            yield return new WaitForSeconds(_warningTime);

            ShowLine(_warningLine, false);
            ShowLine(_laserLine, true);

            yield return new WaitForSeconds(_fireTime);

            ShowLine(_laserLine, false);

            float wait = _totalCooldown - _warningTime - _fireTime;
            if (wait > 0)
                yield return new WaitForSeconds(wait);
        }
    }

    private void ShowLine(LineRenderer line, bool active)
    {
        if (!line) return;

        line.enabled = active;
        if (active)
            UpdateLaserLine(line);
    }

    private void UpdateLaserLine(LineRenderer line)
    {
        Vector2 origin = _firePoint.position;

        Vector2 endPoint = fixedEndPoint;

        line.positionCount = 2;
        line.SetPosition(0, origin);
        line.SetPosition(1, endPoint);
    }
}