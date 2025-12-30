using UnityEngine;

public class Stage2SceneController : MonoBehaviour
{
    void Start()
    {
        StartStage2BGM();
        StratStageSFX();
    }

    public void StartStage2BGM()
    {
        SoundManager.Instance.StopBGMWithFade(0);
        SoundManager.Instance.PlayBGMWithFade("BGM_Stage2");
    }

    public void StratStageSFX()
    {
        SoundManager.Instance.PlaySFX("SFX_Stage_Start");
    }

    public void PlayBossBGM()
    {
        SoundManager.Instance.StopBGMWithFade(0);
        SoundManager.Instance.PlayBGMWithFade("BGM_Boss");
    }
}
