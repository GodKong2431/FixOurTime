using System.Collections;
using UnityEngine;

public class Feather : DamageableTrapBase
{
    [Header("이동")]
    [SerializeField] private float speed = 15f;

    [Header("각도 오프셋")]
    [SerializeField] private float AngleOffset = 30f;

    [Header("움직임 딜레이")]
    [SerializeField] private float _moveDelay  = 1f;

    [Header("생명 주기")]
    [SerializeField] private float lifeTime = 3f;

    
    private bool _stuck;

    private Vector2 _direction;

    private Coroutine _moveCoroutine;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetRandomDirection();
        _moveCoroutine = StartCoroutine(MoveDelay());
    }

    private void OnEnable()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (_stuck) return;

        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            Stick(other.transform);
        }

        StartCoroutine(DestroyAndAlphaChange());
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (_stuck) return;
    }

    void SetRandomDirection()
    {
        float angleOffset = Random.Range(-AngleOffset, AngleOffset);

        _direction = Quaternion.Euler(0f, 0f, angleOffset) * Vector2.down;
        SetRotation(_direction);
    }

    private IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(_moveDelay);
        Destroy(gameObject, lifeTime);
        while (true)
        {
            if (_stuck) yield break;

            transform.position += (Vector3)(_direction * speed * Time.deltaTime);
            yield return null;
        }

    }

    void SetRotation(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(Vector2.down, dir);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Stick(Transform target)
    {
        _stuck = true;
        _direction = Vector2.zero;
        transform.SetParent(target, true);
    }

    private IEnumerator DestroyAndAlphaChange()
    {
        float elapse = 0f;
        Color color =_spriteRenderer.color;

        while(elapse < lifeTime)
        {
            elapse += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapse / lifeTime);
            _spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }

}
