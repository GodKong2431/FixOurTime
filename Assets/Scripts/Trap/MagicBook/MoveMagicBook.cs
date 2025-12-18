using System.Collections;
using UnityEngine;

public class MoveMagicBook : MonoBehaviour
{
    [SerializeField] Vector2 boxSize = new Vector2(0.9f, 0.05f);
    [SerializeField] float boxOffsetY = 0.05f;

    private Vector3 deltaMove;
    private Vector3 _lastPos;

    PointMoveObj _move;

    private void Awake()
    {
        _move = GetComponent<PointMoveObj>();
    }

    private void Start()
    {
        StartCoroutine(AutoMove());
    }

    IEnumerator AutoMove()
    {
        while (true)
        {
            _move.MoveNextPoint();
            _move.ChangeNextPoint();

            yield return null;
        }
    }

    void LateUpdate()
    {
        deltaMove = transform.position - _lastPos;
        _lastPos = transform.position;

        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * boxOffsetY;

        RaycastHit2D hit = Physics2D.BoxCast(origin,boxSize,0f,Vector2.up,0,1<<10);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out Rigidbody2D rb))
                rb.position += (Vector2)deltaMove;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 origin = (Vector2)transform.position + Vector2.up * boxOffsetY;
        Gizmos.DrawWireCube(origin, boxSize);
    }
}
