using System.Collections;
using UnityEngine;

public class PositionSwap : MonoBehaviour
{
    [Header("스왑할 존 두개")]
    [SerializeField] Transform _hotZone;
    [SerializeField] Transform _coolZone;

    [Header("교체주기")]
    [SerializeField] float _interval = 8f;

    private void Start()
    {
        if (_hotZone != null && _coolZone != null)
        {
            StartCoroutine(SwapPos());
        }
    }

    IEnumerator SwapPos()
    {
        while (true)
        {
            yield return new WaitForSeconds(_interval);

            Vector3 tempPos = _hotZone.position;
            _hotZone.position = _coolZone.position;
            _coolZone.position = tempPos;
        }
    }

}
