using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] enum _sliderType { BGM, SFX }
    private _sliderType _type;

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
            _slider.value = SoundManager.instance._bgmVolume;
        }
        else
        {
            _slider.value = SoundManager.instance._sfxVolume;
        }
    }
    //public void SetLevel(float sliderVal)
    //{
    //    _mixer.SetFloat("Volume", Mathf.Log10(sliderVal) * 20);
    //
    //    if (this.gameObject.name == "BgmSlider")
    //    {
    //        SoundManager.instance._bgmVolume = sliderVal;
    //    }
    //
    //    if (this.gameObject.name == "SfxSlider")
    //    {
    //        SoundManager.instance._sfxVolume = sliderVal;
    //    }
    //}

}