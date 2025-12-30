using UnityEngine;

public class Stage1SceneController : MonoBehaviour
{
    void Start()
    {
        StartStage1BGM();
        StratStageSFX();
    }

    public void StartStage1BGM()
    {
        SoundManager.Instance.StopBGMWithFade(0);
        SoundManager.Instance.PlayBGMWithFade("BGM_Stage1");
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
