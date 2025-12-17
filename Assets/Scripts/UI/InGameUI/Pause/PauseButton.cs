using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseButton : MonoBehaviour
{
    public event Action _onPauseClicked;
    public event Action _onResume;

    public void OnClikEscape() 
    { 
        bool ispaneActive = gameObject.activeSelf == false;

        if (ispaneActive == true)
        { 
            _onResume?.Invoke(); 
        }
        else 
        { 
            _onPauseClicked?.Invoke(); 
        } 
    }
    public void OnEscape(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            OnClikEscape();
        }
    }
}