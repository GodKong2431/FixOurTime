using UnityEngine;

[System.Serializable]
public class Boss3DevilData : BossCommonData
{
    [Header("=== 패턴 공통 ===")]
    [Tooltip("패턴 사이 기본 대기 시간")]
    [SerializeField] private float _patternDelay = 1.0f;
    public float PatternDelay => _patternDelay;

    [Header("=== 코어 도트 데미지 ===")]
    [Tooltip("코어에 닿았을 때 한 틱당 들어가는 피해량")]
    [SerializeField] private float _coreTickDamage = 7.5f;
    [Tooltip("코어 도트 데미지 틱 간격(초)")]
    [SerializeField] private float _coreTickInterval = 0.5f;
    [Tooltip("코어 도트 데미지 총 틱 수")]
    [SerializeField] private int _coreTickCount = 4;

    public float CoreTickDamage => _coreTickDamage;
    public float CoreTickInterval => _coreTickInterval;
    public int CoreTickCount => _coreTickCount;

    [Header("=== 블랙홀 ===")]
    [Tooltip("블랙홀 유지 시간(초)")]
    [SerializeField] private float _blackHoleDuration = 5f;
    public float BlackHoleDuration => _blackHoleDuration;

    [Header("=== 손(DevilHand) ===")]
    [Tooltip("손 공격 데미지")]
    [SerializeField] private int _handDamage = 35;
    [Tooltip("손 이동 속도")]
    [SerializeField] private float _handMoveSpeed = 18f;
    [Tooltip("손 공격 속도")]
    [SerializeField] private float _handAttackSpeed = 18f;

    [Tooltip("11자 패턴 준비 시간")]
    [SerializeField] private float _verticalReadyTime = 0.4f;
    [Tooltip("11자 패턴 후 대기 시간")]
    [SerializeField] private float _verticalAfterAttackTime = 0.6f;

    [Tooltip("X자 패턴 준비 시간")]
    [SerializeField] private float _crossReadyTime = 0.4f;
    [Tooltip("X자 패턴 후 대기 시간")]
    [SerializeField] private float _crossAfterAttackTime = 0.6f;

    [Tooltip("회전 패턴 준비 시간")]
    [SerializeField] private float _spiralReadyTime = 0.4f;
    [Tooltip("회전 패턴 후 대기 시간")]
    [SerializeField] private float _spiralAfterAttackTime = 0.6f;
    [Tooltip("회전 궤도 반경")]
    [SerializeField] private float _spiralOffset = 6f;
    [Tooltip("회전 패턴 지속 시간")]
    [SerializeField] private float _spiralDuration = 3f;

    public int HandDamage => _handDamage;
    public float HandMoveSpeed => _handMoveSpeed;
    public float HandAttackSpeed => _handAttackSpeed;
    public float VerticalReadyTime => _verticalReadyTime;
    public float VerticalAfterAttackTime => _verticalAfterAttackTime;
    public float CrossReadyTime => _crossReadyTime;
    public float CrossAfterAttackTime => _crossAfterAttackTime;
    public float SpiralReadyTime => _spiralReadyTime;
    public float SpiralAfterAttackTime => _spiralAfterAttackTime;
    public float SpiralOffset => _spiralOffset;
    public float SpiralDuration => _spiralDuration;

    [Header("=== 창(DevilDarkSpear) ===")]
    [Tooltip("창 공격 데미지")]
    [SerializeField] private int _spearDamage = 30; 
    [Tooltip("창 이동 속도")]
    [SerializeField] private float _spearMoveSpeed = 2f;
    [Tooltip("창이 유지되는 시간(StayTime)")]
    [SerializeField] private float _spearStayTime = 3f;
    [Tooltip("창 공격 후 되돌아가기까지 딜레이")]
    [SerializeField] private float _spearReturnDelay = 3f;

    public int SpearDamage => _spearDamage;
    public float SpearMoveSpeed => _spearMoveSpeed;
    public float SpearStayTime => _spearStayTime;
    public float SpearReturnDelay => _spearReturnDelay;
}
