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

    public Coroutine _checkCoroutine;

    private void Start()
    {
        StartCoroutine(CheckStayTime());
    }

    public IEnumerator CheckStayTime()
    {
        Debug.Log("시작");
        _currentStayTime = 0f;

        while (true)
        {
            RaycastHit2D hit = Physics2D.BoxCast(boxPos.position + (Vector3)boxOffset,boxSize,0f,Vector2.zero,0f,1 << 10);

            if (hit.collider != null)
            {
                Debug.Log(_currentStayTime);
                _currentStayTime += Time.deltaTime;

                if (_currentStayTime >= _stayTime)
                {
                    if (_checkCoroutine != null) yield return null;
                    yield return _checkCoroutine = StartCoroutine(CallDarkSpear());
                    _currentStayTime = 0f;
                }
            }
            else
            {
                Debug.Log("나감");
                _currentStayTime = 0f;
            }

            yield return null;
        }
    }

    public IEnumerator CallDarkSpear()
    {
        _devilDarkSpear.AttackSpear();
        yield return new WaitForSeconds(3);
        _devilDarkSpear.ReturnSpear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxPos.position + (Vector3)boxOffset, boxSize);
    }
}
