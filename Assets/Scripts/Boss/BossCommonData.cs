using UnityEngine;

[System.Serializable]
public class BossCommonData
{
    [Header("Common Stat")]
    [Tooltip("보스 최대 체력")]
    [SerializeField] private float _bossMaxHp = 100f;



    public float BossMaxHp => _bossMaxHp;
}