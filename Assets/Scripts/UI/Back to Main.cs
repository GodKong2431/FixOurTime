using UnityEngine;
using UnityEngine.SceneManagement;

public class BacktoMain : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("LHS_Main");
    }
}
