using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainPanel : MonoBehaviour
{
    //패널 안에 들어있는 버튼들    
    public event Action _onCheckYes;
    public event Action _onCheckNo;

    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ClickYes()
    {
        _onCheckYes?.Invoke();
    }

    public void ClickNo()
    {
        _onCheckNo?.Invoke();
    }

}
