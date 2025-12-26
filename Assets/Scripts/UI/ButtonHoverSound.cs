using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickSound;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _audioSource.Play();
    }

    public void OnCilck()
    {
        SoundManager.instance.PlaySFX(_clickSound);
    }
}
