using UnityEngine;

public class Stage3SceneController : MonoBehaviour
{
    void Start()
    {
        StartStage3BGM();
        StratStageSFX();
    }

    public void StartStage3BGM()
    {
        SoundManager.Instance.StopBGMWithFade(0);
        SoundManager.Instance.PlayBGMWithFade("BGM_Stage3");
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
