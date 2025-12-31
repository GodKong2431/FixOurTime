using UnityEngine;

public class Stage4SceneController : MonoBehaviour
{
    void Start()
    {
        StartStage4BGM();
        StratStageSFX();
    }

    public void StartStage4BGM()
    {
        SoundManager.Instance.StopBGMWithFade(0);
        SoundManager.Instance.PlayBGMWithFade("BGM_Stage4");
    }

    public void StratStageSFX()
    {
        SoundManager.Instance.PlaySFX("SFX_Stage_Start");
    }
}
