using UnityEngine;
using System.Collections;

public class DevilBlackHoleController : MonoBehaviour
{
    [Header("ºí·¢È¦")]
    [SerializeField] private DevilBlackHole _blackHolePrefab;

    [Header("½ºÆù ¼³Á¤")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _duration = 5f;

    [Header("Å©±â ¼³Á¤")]
    [SerializeField] private float _minScale = 0.5f;
    [SerializeField] private float _maxScale = 3f;
    [SerializeField] private float _growTime = 1.5f;
    [SerializeField] private float _shrinkTime = 1f;

    [Header("»¡¾ÆµéÀÌ´Â Èû")]
    [SerializeField] private float _pullSpeed = 5f;   // ¿Ü°û ÃÖ¼Ò Èû

    private DevilBlackHole currentBlackHole;

    public IEnumerator BlackHoleCoroutine()
    {
        currentBlackHole = Instantiate(_blackHolePrefab,_spawnPoint.position,Quaternion.identity);

        currentBlackHole.Initialize(_minScale,_maxScale,_growTime,_shrinkTime, _pullSpeed);

        currentBlackHole.Activate();

        yield return new WaitForSeconds(_duration);

        currentBlackHole.Deactivate();
        currentBlackHole = null;
    }
}