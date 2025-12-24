using UnityEngine;

public class Stage3DevilBoss : BossBase
{
    [Header("Stage3 Devil Data")]
    [SerializeField] private Boss3DevilData _bossData = new Boss3DevilData();
    public Boss3DevilData Data => _bossData;

    [Header("Devil Pattern Controller")]
    [SerializeField] private DevilPatternController _patternController;

    private bool _isActivated = false;

    protected override void Start()
    {
        // 데이터에서 MaxHp 읽어서 보스 체력 세팅
        if (_bossData != null)
        {
            _bossMaxHp = _bossData.BossMaxHp;
        }

        base.Start();

        if (_patternController == null)
        {
            _patternController = GetComponent<DevilPatternController>();
        }

        // 패턴 컨트롤러가 데이터에 접근할 수 있게 초기화
        if (_patternController != null)
        {
            _patternController.Initialize(this);
        }
    }

    /// <summary>
    /// BossZone에서 호출해서 보스를 활성화하는 진입점
    /// </summary>
    public override void ActivateBoss()
    {
        if (_isActivated) return;
        _isActivated = true;

        Debug.Log("[Stage3DevilBoss] 악마 보스 활성화");

        if (_patternController != null)
        {
            _patternController.StartPattern();
        }
    }

    public override void ResetBoss()
    {
        base.ResetBoss();

        _isActivated = false;

        if (_patternController != null)
        {
            _patternController.StopPattern();
        }

        Debug.Log("[Stage3DevilBoss] 악마 보스 리셋");
    }

    /// <summary>
    /// DevilCore에서 호출: 보스 HP 감소 + 디버그 로그
    /// </summary>
    public void ApplyDamage(float damage, float knockbackForce, Vector3 hitPos)
    {
        float before = _currentHp;
        _currentHp = Mathf.Max(_currentHp - damage, 0f);

        Debug.Log($"[Stage3DevilBoss] Damage: {damage}, HP: {before} -> {_currentHp}");

        if (_currentHp <= 0f)
        {
            Die();
        }
    }

    protected override void Die()
    {
        Debug.Log("[Stage3DevilBoss] 악마 보스 사망");

        if (_patternController != null)
        {
            _patternController.StopPattern();
        }

        gameObject.SetActive(false);
    }
}