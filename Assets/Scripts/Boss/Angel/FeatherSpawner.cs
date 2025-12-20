using System.Collections;
using UnityEngine;

public class FeatherSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private GameObject featherPrefab;

    [SerializeField] private float offset = 5f;

    [SerializeField] private int featherCount = 25;

    [SerializeField] private float randomXOffset = 1f;

    [Header("스폰 설정")]
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
        if (featherCount <= 1) yield break;

        while(true)
        {
            yield return _cool;

            float minX = transform.position.x - offset;
            float maxX = transform.position.x + offset;
            float interval = (maxX - minX) / (featherCount - 1);

            for (int i = 0; i < featherCount; i++)
            {
                float x = minX + interval * i;
                x += Random.Range(-randomXOffset, randomXOffset);

                Vector2 spawnPos = new Vector2(x, transform.position.y);
                Instantiate(featherPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
