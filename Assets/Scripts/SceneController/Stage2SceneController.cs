using UnityEngine;

public class Stage2SceneController : MonoBehaviour
{
    void Start()
    {
        EnterStage2();
    }

    private void EnterStage2()
    {
        SoundManager.Instance.StopBGMWithFade(0);
        SoundManager.Instance.PlayBGMWithFade("BGM_Stage2");
        SoundManager.Instance.PlaySFX("SFX_Stage_Start");
        gameObject.SetActive(false);
    }
}
