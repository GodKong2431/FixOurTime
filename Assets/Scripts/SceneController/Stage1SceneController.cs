using UnityEngine;

public class Stage1SceneController : MonoBehaviour
{
    void Start()
    {
        EnterStage1();
    }

    private void EnterStage1()
    {
        SoundManager.Instance.StopBGMWithFade(0);
        SoundManager.Instance.PlayBGMWithFade("BGM_Stage1");
        SoundManager.Instance.PlaySFX("SFX_Stage_Start");
        gameObject.SetActive(false);
    }
}
