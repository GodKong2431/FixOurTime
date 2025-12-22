using System.Collections;
using UnityEngine;

public class FeatherSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private GameObject _featherPrefab;
    [SerializeField] private int _featherCount = 25;

    [Header("스폰 오프셋")]
    [SerializeField] private float _offsetX = 5f;
    [SerializeField] private float _rnadomOffsetX = 1f;
    [SerializeField] private float _rnadomOffsetY = 1f;

    [Header("쿨타임")]
    [SerializeField] private float _coolTime;

    private WaitForSeconds _cool;

    private void Awake()
    {
        _cool = new WaitForSeconds(_coolTime);
    }

    private void Start()
    {
        StartCoroutine(SpawnFeather());
    }


    public IEnumerator SpawnFeather()
    {
        if (_featherCount <= 1) yield break;

        while(true)
        {
            yield return _cool;

            float minX = transform.position.x - _offsetX;
            float maxX = transform.position.x + _offsetX;
            float interval = (maxX - minX) / (_featherCount - 1);

            for (int i = 0; i < _featherCount; i++)
            {
                float x = minX + interval * i;
                x += Random.Range(-_rnadomOffsetX, _rnadomOffsetX);
                float y = transform.position.y + Random.Range(-_rnadomOffsetY, _rnadomOffsetY);

                Vector2 spawnPos = new Vector2(x, y);
                Instantiate(_featherPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
