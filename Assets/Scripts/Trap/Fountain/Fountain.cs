using System.Collections;
using UnityEngine;

public class Fountain : MonoBehaviour
{
    [Header("Effect Interval")]
    [SerializeField] private float _delay = 0.3f;

    private Coroutine _removeEffectCoroutine;
    private WaitForSeconds _removeEffectDelay;

    public DayZone _zone;

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
            if(collision.TryGetComponent(out Player player))
            _removeEffectCoroutine = StartCoroutine(RemoveEffectCoroutine(player));
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
    private IEnumerator RemoveEffectCoroutine(Player player)
    {
        while (true)
        {
            RemoveEffect(player);
            yield return _removeEffectDelay;
        }
    }

    private void RemoveEffect(Player player)
    {
        player.RemoveDebuffByName("더움");
        player.RemoveDebuffByName("탈진");
        _zone._stayTimer = 0;

        Debug.Log("이펙트 제거 실행됨");
    }
    
    //분수 활성화/비활성화
    public void ChangeActiveFountain()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
