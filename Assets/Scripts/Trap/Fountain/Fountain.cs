using System.Collections;
using UnityEngine;

public class Fountain : MonoBehaviour
{
    [Header("Effect Interval")]
    [SerializeField] private float _delay = 0.3f;

    private Coroutine _removeEffectCoroutine;
    private WaitForSeconds _removeEffectDelay;

    private void Awake()
    {
        _removeEffectDelay = new WaitForSeconds(_delay);
    }

    //콜라이더 들어올 시 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (_removeEffectCoroutine == null)
        {
            Debug.Log("이펙트 제거 코루틴 실행");
            _removeEffectCoroutine = StartCoroutine(RemoveEffectCoroutine(collision.gameObject));
        }
    }

    //콜라이더 나갈시 종료
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        Debug.Log("이펙트 제거 코루틴 종료");
        if (_removeEffectCoroutine != null)
        {
            StopCoroutine(_removeEffectCoroutine);
            _removeEffectCoroutine = null;
        }
    }

    //이펙트 비활성화 0.3초마다 실행
    private IEnumerator RemoveEffectCoroutine(GameObject player)
    {
        while (true)
        {
            RemoveEffect(player);
            yield return _removeEffectDelay;
        }
    }

    //이펙트 비활성화 구현 해야됨
    private void RemoveEffect(GameObject player)
    {
        Debug.Log("이펙트 제거 실행됨");
        //‘더움’ 상태일 경우: 상태 이상 타이머를 초기화 함
        //‘탈진‘ 상태일 경우: 상태 이상이 즉시 해제되며, 분수 효과 발둥 후 1.0초 뒤 다시 ‘더움‘ 상태로 진입
    }
    
    //분수 활성화/비활성화
    public void ChangeActiveFountain()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
