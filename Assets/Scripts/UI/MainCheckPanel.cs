using System;
using UnityEngine;

public class MainCheckPanel : MonoBehaviour
{
    public event Action _mainCheckYes;
    public event Action _mainCheckNo;

    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void MainClickYes()
    {
        _mainCheckYes?.Invoke();
    }

    public void MainClickNo()
    {
        _mainCheckNo?.Invoke();
    }
}
