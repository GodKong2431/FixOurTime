using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseButton : MonoBehaviour
{
    
    public void OnClickPause()
    {
        GameManager.Instance.TogglePause();
    }
}