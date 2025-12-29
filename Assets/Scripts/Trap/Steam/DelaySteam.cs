using System.Collections;
using UnityEngine;

public class DelaySteam : MonoBehaviour
{
    [SerializeField] private GameObject _steam;
    [SerializeField] private float _duration;
    [SerializeField] private float _delay;

    WaitForSeconds Duration;
    WaitForSeconds Delay;

    private void Awake()
    {
        Duration = new WaitForSeconds(_duration);
        Delay = new WaitForSeconds(_delay);
    }

    private void Start()
    {

        StartCoroutine(DelaySteamCoroutine());
    }

    private IEnumerator DelaySteamCoroutine()
    {
        while (true)
        {

            _steam.SetActive(true);
            yield return Duration;

            yield return StartCoroutine(_steam.GetComponent<Steam>().OffSteam());
            _steam.SetActive(false);
            yield return Delay;
        }
    }
}
