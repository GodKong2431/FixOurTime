using System.Collections;
using UnityEngine;

public class BookTrap : MonoBehaviour
{
    enum BookState { Active, flash, Deactivate } //활성 , 경고 , 비활성화

    //기본상태는 열거형 0번 Active
    private BookState _currentState;

    private SpriteRenderer _spriteRenderer; //투명도 조절용
    private Collider2D _collider;   //판정 콜라이더 온오프용

    private float _timer; //상태변화 쿨타임

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        //상태변화 스위치
        switch (_currentState)
        {
            case BookState.Active:
                if (_timer >= 4f)   //4초마다 비활성화전 점멸상태로
                {
                    ChangeState(BookState.flash);
                }
                break;

            case BookState.flash:
                if (_timer >= 1f)    //1초뒤 비활성화
                {
                    ChangeState(BookState.Deactivate);
                }
                break;

            case BookState.Deactivate:
                if (_timer >= 3f)   //3초뒤 원상복귀
                {
                    ChangeState(BookState.Active);
                }
                break;
        }
    }

    //상태변화에 따른 행동들
    private void ChangeState(BookState newState)
    {
        _currentState = newState;
        _timer = 0f;    //쿨타임초기화

        switch (newState)
        {
            case BookState.Active:
                ActiveState();
                break;

            case BookState.flash:
                FlashState();
                break;

            case BookState.Deactivate:
                DeactiveState();
                break;
        }
    }

    private void ActiveState()
    {
        //활성화니깐 투명도 없애기
        Color color = _spriteRenderer.color;
        color.a = 1f;
        _spriteRenderer.color = color;

        //콜라이더 활성화
        _collider.enabled = true;
    }

    private void DeactiveState()
    {
        Color color = _spriteRenderer.color;
        color.a = 0f;
        _spriteRenderer.color = color;

        _collider.enabled = false;
    }

    private void FlashState()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        Color color = _spriteRenderer.color;
        float timer = 0f;   
        float duration = 1f; 
        bool repeat = true; //반복 체크용 변수

        while (timer < duration) //1초동안 반복
        {

            if (repeat)
            {
                color.a = 1f;  
                repeat = false; 
            }
            else
            {
                color.a = 0f;  
                repeat = true;
            }

            
            _spriteRenderer.color = color;

            // 0.2초 대기
            yield return new WaitForSeconds(0.1f);

            // 타이머 증가
            timer += 0.1f;

        }

        //투명도 복구
        color.a = 1f;
        _spriteRenderer.color = color;

    }
}
