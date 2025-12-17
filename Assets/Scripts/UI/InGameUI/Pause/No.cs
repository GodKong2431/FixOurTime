using System;
using UnityEngine;

public class No : MonoBehaviour
{
    public event Action _no;

    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClik()
    {
        _no?.Invoke();
    }
}
