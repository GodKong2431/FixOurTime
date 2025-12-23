using System.Collections;
using UnityEngine;

public class Stage3AngelPlatform : MonoBehaviour
{
    [Header("시간 제한")]
    [SerializeField] private float _stayTime;

    [Header("보스 및 스킬")]
    [SerializeField] private Angel _angel;
    [SerializeField] private Judgment _judgment;

    [Header("이동시간")]
    [SerializeField] private float _moveTime = 1f;
    [SerializeField] private float _sleep = 1f;


    private float _moveDistance;
    

    private float _currentStayTime = 0f;

    private Vector2 _initPos;
    private Vector2 _lastPos;

    private Collider2D _col;

    private WaitForSeconds _sleepTime;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _sleepTime = new WaitForSeconds(_sleep);
        _initPos = transform.position;
        _moveDistance = _col.bounds.size.x * 0.5f;
    }

    private void Start()
    {
        StartCoroutine(MovePlatform());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            _lastPos = collision.transform.position - transform.position;

            _angel.StayTimeController(this);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            _lastPos = collision.transform.position - transform.position;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            _lastPos = collision.transform.position - transform.position;
        }
    }


    public IEnumerator CheckStayTime()
    {
        _currentStayTime = 0f;

        while (true)
        {
            _currentStayTime += Time.deltaTime;

            if (_currentStayTime >= _stayTime)
            {
                CallJudgment();
                _currentStayTime = 0f;
            }

            yield return null;
        }
    }

    private void CallJudgment()
    {
        Vector2 worldPos = (Vector2)transform.position + _lastPos;
        _judgment.StartJudgment(worldPos + new Vector2(0, -1f));
    }

    private IEnumerator MovePlatform()
    {
        while (true)
        {
            int moveDir = Random.value < 0.5f ? -1 : 1;

            Vector2 startPos = transform.position;
            float targetX = startPos.x + moveDir * _moveDistance;

            float minX = _initPos.x - _moveDistance;
            float maxX = _initPos.x + _moveDistance;

            Vector2 targetPos = new Vector2(Mathf.Clamp(targetX, minX, maxX), startPos.y);

            float currentTime = 0f;

            while (currentTime < _moveTime)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / _moveTime;

                transform.position = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
            yield return _sleepTime;
        }
    }
}
