using UnityEngine;

public class Stage4SceneController : MonoBehaviour
{
    void Start()
    {
        EnterStage4();
    }

    private void EnterStage4()
    {
        SoundManager.Instance.PlaySFX("SFX_Stage_Start");
        gameObject.SetActive(false);
    }
}
