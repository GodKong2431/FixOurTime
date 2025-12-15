using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("LHS_InGame"); // 추후 인게임씬이름으로 변경
    }
}
