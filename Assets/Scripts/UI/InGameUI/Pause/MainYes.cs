using System;
using UnityEngine;

public class MainYes : MonoBehaviour
{
    public event Action _mainYes;
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
        _mainYes?.Invoke();
    }
}
