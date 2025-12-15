using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseButton : MonoBehaviour
{
    public event Action _onPauseClicked;

    public void ClickPause()
    {
        _onPauseClicked?.Invoke();   //퍼즈 알림
    }

    public void OnEscape(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ClickPause();
        }
    }
}