using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
}
