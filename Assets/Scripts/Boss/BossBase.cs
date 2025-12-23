using UnityEngine;
using System.Collections;

public abstract class BossBase : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected Transform _playerTarget;
    [SerializeField] protected float _bossMaxHp = 100f;

    protected float _currentHp;
    // 외부에서 체력바 UI 등이 읽을 수 있게 프로퍼티 제공
    public float CurrentHp => _currentHp;

    protected BossState _currentState;

    protected virtual void Start()
    {
        _currentHp = _bossMaxHp;

        // 플레이어 자동 찾기 (안전장치)
        if (_playerTarget == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                _playerTarget = p.transform;
            }
        }
    }

    // 공통 피격 로직
    public virtual void TakeDamage(float damage)
    {
        _currentHp -= damage;
        Debug.Log($"[{gameObject.name}] HP: {_currentHp}");

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    // 상태 전환 (FSM)
    public IEnumerator ChangeState(BossState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = newState;
        _currentState.Enter();
        yield return StartCoroutine(_currentState.Execute());
    }

    // 자식들이 반드시 구현해야 할 추상 함수
    protected abstract void Die();
    public abstract void ActivateBoss();

    // 리셋 기능 (필요시 구현)
    public virtual void ResetBoss()
    {
        StopAllCoroutines();
        _currentHp = _bossMaxHp;
        _currentState = null;
    }
}