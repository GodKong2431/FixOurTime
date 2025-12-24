using UnityEngine;
using System.Collections;

public class DevilBlackHoleController : MonoBehaviour
{
    [Header("블랙홀")]
    [SerializeField] private DevilBlackHole _blackHolePrefab;

    [Header("코어")]
    [SerializeField] private DevilCore _devilCore;

    [Header("스폰 설정")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _defaultDuration = 5f;

    [Header("크기 설정")]
    [SerializeField] private float _minScale = 0.5f;
    [SerializeField] private float _maxScale = 3f;
    [SerializeField] private float _growTime = 1.5f;
    [SerializeField] private float _shrinkTime = 1f;

    [Header("빨아들이는 힘")]
    [SerializeField] private float _pullSpeed = 5f;   // 외곽 최소 힘

    private DevilBlackHole currentBlackHole;

    private Boss3DevilData _data;

    private void Awake()
    {
        var boss = GetComponentInParent<Stage3DevilBoss>();
        if (boss != null)
        {
            _data = boss.Data;
        }
    }

    public IEnumerator BlackHoleCoroutine()
    {
        _devilCore.SetBlackHoleActive(true);
        currentBlackHole = Instantiate(_blackHolePrefab,_spawnPoint.position,Quaternion.identity);

        currentBlackHole.Initialize(_minScale,_maxScale,_growTime,_shrinkTime, _pullSpeed);

        currentBlackHole.Activate();

        float duration = _data != null ? _data.BlackHoleDuration : _defaultDuration;
        yield return new WaitForSeconds(duration);

        _devilCore.SetBlackHoleActive(false);
        currentBlackHole.Deactivate();
        currentBlackHole = null;
    }

    private void OnDisable()
    {
        // 보스가 죽으면 소환해둔 블랙홀도 같이 삭제
        if (currentBlackHole != null)
        {
            Destroy(currentBlackHole.gameObject);
            currentBlackHole = null;
        }
    }
}