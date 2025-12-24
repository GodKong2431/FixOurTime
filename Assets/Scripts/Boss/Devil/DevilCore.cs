using System.Collections;
using UnityEngine;

public class DevilCore : MonoBehaviour, IDamageable
{
    private Stage3DevilBoss _boss;
    private Boss3DevilData _data;

    private bool _blackHoleActive;
    private bool _damageRunning;

    private void Awake()
    {
        _boss = GetComponentInParent<Stage3DevilBoss>();
        if (_boss != null)
        {
            _data = _boss.Data;
        }
    }

    public void SetBlackHoleActive(bool active)
    {
        _blackHoleActive = active;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_blackHoleActive) return;
        if (!other.CompareTag("Player")) return;
        if (other.TryGetComponent(out Player player))
        {
            StartCoroutine(CoreDamageCoroutine(player));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_blackHoleActive || _damageRunning) return;
        if (!other.CompareTag("Player")) return;
        if (other.TryGetComponent(out Player player))
        {
            StartCoroutine(CoreDamageCoroutine(player));
        }
    }

    private IEnumerator CoreDamageCoroutine(Player player)
    {
        _damageRunning = true;

        float damage = 7.5f;
        float interval = 0.5f;
        int ticks = 4;

        if (_data != null)
        {
            damage = _data.CoreTickDamage;
            interval = _data.CoreTickInterval;
            ticks = _data.CoreTickCount;
        }

        for (int i = 0; i < ticks; i++)
        {
            player.TakeDamage(damage, 0, transform.position);
            yield return new WaitForSeconds(interval);
        }

        _damageRunning = false;
    }

    // 플레이어가 코어를 공격했을 때 → 보스 HP 감소
    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        if (_boss == null)
        {
            _boss = GetComponentInParent<Stage3DevilBoss>();
            if (_boss != null)
            {
                _data = _boss.Data;
            }
        }

        if (_boss == null)
        {
            Debug.LogWarning("[DevilCore] Stage3DevilBoss를 찾을 수 없습니다.");
            return;
        }

        _boss.ApplyDamage(damage, KnockbackForce, hitPos);
    }
}