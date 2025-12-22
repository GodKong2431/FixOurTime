using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    void Start()
    {
        //버튼에 연결시키기
        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            
            btn.onClick.AddListener(() => {
                if (SceneChanger.Instance != null)
                {
                    SceneChanger.Instance.LoadGameFromSave();
                }
                else
                {
                    Debug.LogError("SceneChanger 인스턴스를 찾을 수 없습니다");
                }
            });
        }
    }
}
