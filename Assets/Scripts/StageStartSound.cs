using UnityEngine;

public class StageStartSound : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlaySFX("SFX_Stage_Start");
        gameObject.SetActive(false);
    }
}
