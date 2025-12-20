using System;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public event Action _onGameStart;

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
        _onGameStart?.Invoke();
    }
}
