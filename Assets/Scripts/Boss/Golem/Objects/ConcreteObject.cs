using UnityEngine;
using System.Collections;

public class ConcreteObject : MonoBehaviour
{
    private BoxCollider2D _col;

    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        _col.isTrigger = true;
    }

    public void Initialize(bool isHorizontal, Vector3 mapCenter, float stayDuration, float moveDuration)
    {
        Vector3 targetPos = transform.position;
        if (isHorizontal) 
        {
            targetPos.x = mapCenter.x;
        }
        else 
        {
            targetPos.y = mapCenter.y;
        }

        StartCoroutine(ProcessRoutine(targetPos, stayDuration, moveDuration));
    }

    private IEnumerator ProcessRoutine(Vector3 target, float stayDuration, float moveDuration)
    {
        // 1. 이동 
        float t = 0;
        Vector3 start = transform.position;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, t / moveDuration);
            yield return null;
        }

        // 2. 변환
        _col.isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("Ground");

        // 3. 유지
        yield return new WaitForSeconds(stayDuration);

        // 4. 복귀 및 소멸 
        _col.isTrigger = true;
        t = 0;
        float returnTime = 1.0f; 
        while (t < returnTime)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(target, start, t / returnTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_col.isTrigger && collision.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(10, 5f, transform.position);
        }
    }
}