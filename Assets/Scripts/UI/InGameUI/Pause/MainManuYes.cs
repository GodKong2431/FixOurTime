using System;
using UnityEngine;

public class MainManuYes : MonoBehaviour
{
    public event Action _mainManuYes;

    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        _mainManuYes?.Invoke();
    }
}
