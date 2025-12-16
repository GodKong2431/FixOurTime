using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{    
    [SerializeField] private AudioClip _audioClip;
    public void OnClick()
    {
        SoundManager.instance.PlayBGM(_audioClip);
        SceneManager.LoadScene("LHS_InGame"); // 추후 인게임씬이름으로 변경
    }
}
