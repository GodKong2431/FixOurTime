using UnityEngine;
using System.Collections;
public class BossObjectManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _scrapPrefab;               // 고철 프리팹
    [SerializeField] private GameObject _concreteHorizontalPrefab;  // 가로 콘크리트
    [SerializeField] private GameObject _concreteVerticalPrefab;    // 세로 콘크리트
    [SerializeField] private GameObject _weaknessPrefab;            // 약점 프리팹

    // 고철 생성 및 초기화
    public void SpawnScrap(Vector3 spawnPos)
    {
        GameObject go = Instantiate(_scrapPrefab, spawnPos, Quaternion.identity);
        ScrapObject scrap = go.GetComponent<ScrapObject>();

        if (scrap != null)
        {
            // 왼쪽으로 날아가도록 초기화
            scrap.Initialize(Vector3.left);
        }
    }

    // 콘크리트 생성 (가로/세로 구분)
    public void SpawnConcrete(Vector3 spawnPos, bool isHorizontal)
    {
        GameObject prefab = isHorizontal ? _concreteHorizontalPrefab : _concreteVerticalPrefab;
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);

        ConcreteObject concrete = go.GetComponent<ConcreteObject>();
        if (concrete != null)
        {
            concrete.Initialize(isHorizontal);
        }
    }

    // 약점 생성
    public GameObject SpawnWeakness(Vector3 spawnPos)
    {
        return Instantiate(_weaknessPrefab, spawnPos, Quaternion.identity);
    }
}