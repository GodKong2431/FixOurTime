using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;

    public void ChangeHpBar(float currentHp, float maxHp)
    {
        _hpBar.maxValue = maxHp;
        _hpBar.value = currentHp;
        Debug.Log($"HP UI °»½Å current:{currentHp}, max:{maxHp}, sliderMax:{_hpBar.maxValue}");
    }

}
