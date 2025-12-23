using UnityEngine;

[System.Serializable]
public class Boss2Data : BossCommonData
{
    [Header("=== Paper Crane (종이학) ===")]
    [Tooltip("종이학 생성 주기 (5초)")]
    [SerializeField] private float _craneSpawnInterval = 5.0f;

    [Tooltip("플레이어 속도 배율 (1.2배)")]
    [SerializeField] private float _craneSpeedMultiplier = 1.2f;

    [Tooltip("기본 플레이어 속도 (기준값)")]
    [SerializeField] private float _basePlayerSpeed = 5.0f;
    [Tooltip("속박 지속 시간 (3초)")]
    [SerializeField] private float _bindDuration = 3.0f;
    [Tooltip("속박 풀리지 않을 시 데미지 (50)")]
    [SerializeField] private float _bindFailDamage = 50f;

    [Header("=== Major Book (전공책 - 공격용) ===")]
    [Tooltip("조준 시간 (1초)")]
    [SerializeField] private float _bookAimTime = 1.0f;
    [Tooltip("발사 전 뜸들이는 시간 (0.5초)")]
    [SerializeField] private float _bookFireDelay = 0.5f;
    [Tooltip("전공책 이동 속도")]
    [SerializeField] private float _bookMoveSpeed = 25f;
    [Tooltip("전공책 넉백 (5)")]
    [SerializeField] private float _bookKnockback = 5.0f;
    [Tooltip("전공책 직격 데미지 (100)")]
    [SerializeField] private float _bookDirectDamage = 100f;
    [Tooltip("전공책 주변 도트 데미지 (초당 20)")]
    [SerializeField] private float _bookDotDamage = 20f;
    [Tooltip("벽이나 바닥 충돌 후 유지 시간 (3초)")]
    [SerializeField] private float _bookDurationAfterHit = 3.0f;

    [Header("=== Fox (여우) ===")]
    [Tooltip("여우의 이동 속도(6.0)")]
    [SerializeField] private float _foxMoveSpeed = 6.0f;

    [Tooltip("그림자 상태 이동 속도 배율 (1.5배)")]
    [SerializeField] private float _foxShadowSpeedMultiplier = 1.5f;

    [Tooltip("기믹 시작 후 여우 등장 시간 (5초)")]
    [SerializeField] private float _foxSpawnDelay = 5.0f;
    [Tooltip("책 먹는 시간 (3초)")]
    [SerializeField] private float _foxEatDuration = 3.0f;

    [Tooltip("책 먹을 때 플레이어 감지 거리 (4.0)")]
    [SerializeField] private float _foxDetectRange = 4.0f;

    [Tooltip("물기 데미지 (5)")]
    [SerializeField] private float _foxBiteDamage = 5f;
    [Tooltip("물린 후 도트 데미지 (초당 5)")]
    [SerializeField] private float _foxBiteDotDamage = 5f;
    [Tooltip("그림자 광역 공격 데미지 (70)")]
    [SerializeField] private float _foxAoeDamage = 70f;
    [Tooltip("그림자 광역 공격 넉백 거리 (10)")]
    [SerializeField] private float _foxShadowKnockback = 10.0f;
    [Tooltip("그림자 공격 대기 시간 (2초)")]
    [SerializeField] private float _foxAoeDelay = 2.0f;

    [Header("=== Gimmick (아이템) ===")]
    [Tooltip("기믹 성공 시 보스 피해량 (10)")]
    [SerializeField] private float _gimmickSuccessDamage = 10f;
    [Tooltip("아이템 숨김 대기 시간 (2초)")]
    [SerializeField] private float _itemHideDelay = 2.0f;
    [Tooltip("기믹 한 사이클 종료 후 다음 기믹까지 대기 시간")]
    [SerializeField] private float _gimmickInterval = 5.0f;

    // 프로퍼티
    public float CraneSpawnInterval => _craneSpawnInterval;
    public float CraneSpeedMultiplier => _craneSpeedMultiplier;
    public float BasePlayerSpeed => _basePlayerSpeed;
    public float BindDuration => _bindDuration;
    public float BindFailDamage => _bindFailDamage;

    public float BookAimTime => _bookAimTime;
    public float BookFireDelay => _bookFireDelay;
    public float BookMoveSpeed => _bookMoveSpeed;
    public float BookKnockback => _bookKnockback;
    public float BookDirectDamage => _bookDirectDamage;
    public float BookDotDamage => _bookDotDamage;
    public float BookDurationAfterHit => _bookDurationAfterHit;

    public float FoxMoveSpeed => _foxMoveSpeed;
    public float FoxShadowSpeedMultiplier => _foxShadowSpeedMultiplier;

    public float FoxSpawnDelay => _foxSpawnDelay;
    public float FoxEatDuration => _foxEatDuration;
    public float FoxDetectRange => _foxDetectRange;
    public float FoxBiteDamage => _foxBiteDamage;
    public float FoxBiteDotDamage => _foxBiteDotDamage;
    public float FoxAoeDamage => _foxAoeDamage;
    public float FoxShadowKnockback => _foxShadowKnockback;
    public float FoxAoeDelay => _foxAoeDelay;

    public float GimmickSuccessDamage => _gimmickSuccessDamage;
    public float ItemHideDelay => _itemHideDelay;
    public float GimmickInterval => _gimmickInterval;
}