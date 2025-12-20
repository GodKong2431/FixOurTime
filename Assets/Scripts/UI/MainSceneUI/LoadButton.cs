using System;
using UnityEngine;

public class LoadButton : MonoBehaviour
{
    public event Action _onLoad;
    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Onclick()
    {
        _onLoad?.Invoke();
    }
}
