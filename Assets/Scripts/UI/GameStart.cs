using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("PlayScene");
    }
}
