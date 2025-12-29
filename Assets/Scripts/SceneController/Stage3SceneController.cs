using UnityEngine;

public class Stage3SceneController : MonoBehaviour
{
    void Start()
    {
        EnterStage3();
    }

    private void EnterStage3()
    {
        SoundManager.Instance.PlaySFX("SFX_Stage_Start");
        gameObject.SetActive(false);
    }
}
