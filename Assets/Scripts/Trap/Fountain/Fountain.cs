using System.Collections;
using UnityEngine;

public class Fountain : MonoBehaviour
{
    [Header("활성화 시간 설정")]
    [SerializeField] float _activeDuration = 8f;
    [SerializeField] float _deactDuration = 8f;
    [SerializeField] float _startDelay = 0f;

    [Header("이펙트 제거 주기")]
    [SerializeField] private float _delay = 0.3f;
    [SerializeField] private DayZone _zone;

    private Coroutine _removeEffectCoroutine;
    private WaitForSeconds _removeEffectDelay;

    private Animator _anim;

    private void Awake()
    {
        _removeEffectDelay = new WaitForSeconds(_delay);
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(FountainActiveRoutine());
    }
    IEnumerator FountainActiveRoutine()
    {
        if(_startDelay > 0)
        {
            yield return new WaitForSeconds(_startDelay);
        }

        while (true)
        {
            SetFountainActive(true);
            yield return new WaitForSeconds(_activeDuration);

            SetFountainActive(false);
            yield return new WaitForSeconds(_deactDuration);
        }
    }
    private void SetFountainActive(bool isAct)
    {
        _anim.SetBool("On", isAct);
        GetComponent<BoxCollider2D>().enabled = isAct;
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
        player.RemoveEffectByName("더움");
        player.RemoveEffectByName("탈진");
        _zone._stayTimer = 0;

        Debug.Log("이펙트 제거 실행됨");
    }
    
}
