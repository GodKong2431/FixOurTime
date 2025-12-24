using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BossUIManager : MonoBehaviour
{
    [Header("UI Components")]
    // 상단에 배치할 이미지 슬롯 4개 (Inspector에서 연결)
    [SerializeField] private Image[] _targetItemSlots;

    [Header("Settings")]
    [Tooltip("획득 완료된 슬롯의 색상")]
    [SerializeField] private Color _collectedColor = Color.gray;

    private Color _defaultColor = Color.white;

    private void Awake()
    {
        HideUI();
    }

    // 정답 목록을 보여줄 때 호출 (초기화)
    public void ShowTargetItems(List<Sprite> targets)
    {
        for (int i = 0; i < _targetItemSlots.Length; i++)
        {
            if (i < targets.Count)
            {
                _targetItemSlots[i].gameObject.SetActive(true);
                _targetItemSlots[i].sprite = targets[i];
                _targetItemSlots[i].color = _defaultColor; // 색상 초기화
            }
            else
            {
                _targetItemSlots[i].gameObject.SetActive(false);
            }
        }
    }

    // 획득한 아이템의 이미지를 받아와서 해당 슬롯 비활성화
    public void MarkItemAsCollected(Sprite collectedSprite)
    {
        foreach (var slot in _targetItemSlots)
        {
            // 슬롯이 켜져 있고, 이미지가 같고, 아직 안 먹은(흰색) 상태라면
            if (slot.gameObject.activeSelf && slot.sprite == collectedSprite && slot.color == _defaultColor)
            {
                slot.color = _collectedColor; // 획득 처리 (회색)
                return; // 하나만 처리하고 종료
            }
        }
    }

    public void HideUI()
    {
        foreach (var slot in _targetItemSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }
}