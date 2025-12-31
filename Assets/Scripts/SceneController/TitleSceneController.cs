using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    void Start()
    {
        EnterTitle();
    }

    private void EnterTitle()
    {
        SoundManager.Instance.StopBGMWithFade(0);
        SoundManager.Instance.PlayBGMWithFade("BGM_FixOurTime");
    }
}
