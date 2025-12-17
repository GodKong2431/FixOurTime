using System;
using UnityEngine;

public class QuitManuYes : MonoBehaviour
{
    public event Action _quitManuYes;

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
        _quitManuYes?.Invoke();
    }
}
