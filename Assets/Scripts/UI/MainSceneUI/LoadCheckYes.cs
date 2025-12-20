using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCheckYes : MonoBehaviour
{
    public event Action _onLoadYes;
    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClickYes()
    {
        _onLoadYes?.Invoke();
    }
}
