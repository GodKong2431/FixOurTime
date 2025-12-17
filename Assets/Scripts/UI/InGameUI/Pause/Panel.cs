using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Panel : MonoBehaviour
{
    //패널 안에 들어있는 버튼들
    public event Action _onResume;
    public event Action _onMainManu;
    public event Action _onQuitGame;

    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ClickResume()
    {
        _onResume?.Invoke();
    }

    public void ClickMainManu()
    {
        _onMainManu?.Invoke();
    }

    public void ClickQuitGame()
    {
        _onQuitGame?.Invoke();
    }

    public void OnEscape(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ClickResume();
        }
    }
}
