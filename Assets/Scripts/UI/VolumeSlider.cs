using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    enum _sliderType { BGM, SFX }
    [SerializeField] private _sliderType _type;

    private Slider _slider;

    private void Awake()
    {
        //컴포넌트 정보 먼저 가져오고
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {        
        if (_type == _sliderType.BGM)
        {
            _slider.value = SoundManager.Instance._bgmVolume;
        }
        else
        {
            _slider.value = SoundManager.Instance._sfxVolume;
        }
    }
    public void SetLevel(float value)
    {
        if (_type == _sliderType.BGM)
            SoundManager.Instance.UpdateBgmVolume(value);
        else
            SoundManager.Instance.UpdateSfxVolume(value);
    }

}