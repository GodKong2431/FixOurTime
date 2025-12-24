using System.Collections;
using UnityEngine;

public class DevilCore : MonoBehaviour, IDamageable
{
    private bool _blackHoleActive;
    private bool _damageRunning;

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
        if(other.TryGetComponent(out Player player))
        {
            StartCoroutine(CoreDamageCoroutine(player));
        }
    }

    private IEnumerator CoreDamageCoroutine(Player player)
    {
        _damageRunning = true;

        int ticks = 4;

        for (int i = 0; i < ticks; i++)
        {
            player.TakeDamage(7.5f, 0, transform.position);
            yield return new WaitForSeconds(0.5f);
        }

        _damageRunning = false;
    }

    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        
    }
}
