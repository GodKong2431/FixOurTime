using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameYesBtn : MonoBehaviour
{
    
    public void GoStage1()
    {
        SceneChanger.Instance.ChangeScene("stage1", false);
    }
}
