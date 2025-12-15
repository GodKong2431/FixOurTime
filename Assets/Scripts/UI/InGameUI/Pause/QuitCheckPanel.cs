using System;
using UnityEngine;

public class QuitCheckPanel : MonoBehaviour
{
    public event Action _quitCheckYes;
    public event Action _quitCheckNo;

    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void QuitClickYes()
    {
        _quitCheckYes?.Invoke();
    }

    public void QuitClickNo()
    {
        _quitCheckNo?.Invoke();
    }

}
