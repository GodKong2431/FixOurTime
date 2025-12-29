using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    //[SerializeField] private AudioSource _audioSource;
    //[SerializeField] private AudioClip _clickSound;
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFXOneShot("SFX_UI_ButtonHover");
    }

    public void OnCilck()
    {
        SoundManager.Instance.PlaySFX("SFX_UI_ButtonClick");
    }
}
