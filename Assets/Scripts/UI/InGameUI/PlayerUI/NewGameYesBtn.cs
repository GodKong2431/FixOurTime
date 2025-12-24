using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameYesBtn : MonoBehaviour
{
    
    public void GoStage1()
    {
        SceneChanger.Instance.StartNewGame("stage1");
    }
}
