using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    void Start()
    {
        EnterTitle();
    }

    private void EnterTitle()
    {
        SoundManager.Instance.PlayBGMWithFade("BGM_FixOurTime");
    }
}
