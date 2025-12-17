using System;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    public event Action _mainManuButton;
    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void MainClick()
    {
        _mainManuButton?.Invoke();
    }
}
