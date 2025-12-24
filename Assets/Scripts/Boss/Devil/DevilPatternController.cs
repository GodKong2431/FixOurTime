using System.Collections;
using UnityEngine;

public class DevilPatternController : MonoBehaviour
{
    [Header("패턴 사이 딜레이")]
    [SerializeField] private float _patternDelay = 1.0f;


    private DevilHandController _handPattern;
    private DevilDarkSpearController _darkSpearController;
    private DevilBlackHoleController _blackHoleController;

    private Coroutine _patternCoroutine;
    private WaitForSeconds _delay;

    private void Awake()
    {
        _handPattern = GetComponent<DevilHandController>();
        _darkSpearController = GetComponent<DevilDarkSpearController>();
        _blackHoleController = GetComponent<DevilBlackHoleController>();
        _delay = new WaitForSeconds(_patternDelay);
    }

    private void Start()
    {
        StartCoroutine(PatternA());
    }

    public void StartPattern()
    {
        if (_patternCoroutine != null) StopCoroutine(_patternCoroutine);

        switch(Random.Range(0, 3))
        {
            case 0:
                _patternCoroutine = StartCoroutine(PatternA());
                break;
            case 1:
                PatternB();
                break;
            case 2:
                _patternCoroutine = StartCoroutine(PatternC());
                break;
        }
    }


    IEnumerator PatternA()
    {
        yield return StartCoroutine(_blackHoleController.BlackHoleCoroutine());
        yield return _delay;
        yield return StartCoroutine(_handPattern.CrossPattern());
        yield return _delay;
        StartCoroutine(_handPattern.VerticalPattern());
        yield return StartCoroutine(_darkSpearController.CallDarkSpear());

    }
    private void PatternB()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                _patternCoroutine = StartCoroutine(PatternB_1());
                break;
            case 1:
                _patternCoroutine = StartCoroutine(PatternB_2());
                break;
        }
    }
    IEnumerator PatternB_1()
    {
        yield return StartCoroutine(_handPattern.CrossPattern());
        yield return _delay;
        yield return StartCoroutine(_handPattern.SpiralPattern());
    }
    IEnumerator PatternB_2()
    {
        yield return StartCoroutine(_handPattern.VerticalPattern());
        yield return _delay;
        yield return StartCoroutine(_handPattern.SpiralPattern());
    }
    IEnumerator PatternC()
    {
        StartCoroutine(_blackHoleController.BlackHoleCoroutine());
        yield return StartCoroutine(_darkSpearController.CallDarkSpear());
    }
}
