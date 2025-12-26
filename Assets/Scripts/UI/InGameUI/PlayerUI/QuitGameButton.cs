using System;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameButton : MonoBehaviour
{
    public event Action _quitGameButton;   

    public void Show()
    {
        gameObject.SetActive(true); //씬 기본값으로 비활성화시켜둔 팝업 켜기
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void QuitClick()
    {
        Application.Quit();
    }
    void Start()
    {
        // 1. 버튼 컴포넌트 가져오기
        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            // 2. 클릭 시 GameManager의 QuitGame 메서드 호출
            btn.onClick.AddListener(() => {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.QuitGame();
                }
                else
                {
                    Debug.LogError("GameManager를 찾을 수 없습니다!");
                }
            });
        }
    }

}
