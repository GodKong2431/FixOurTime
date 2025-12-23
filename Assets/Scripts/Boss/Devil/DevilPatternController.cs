using System.Collections;
using UnityEngine;

public class DevilPatternController : MonoBehaviour
{
    [Header("패턴 사이 딜레이")]
    [SerializeField] private float _patternDelay = 1.0f;


    private DevilHandController _handPattern;
    private DevilDarkSpearController _darkSpearController;

    private Coroutine _patternCoroutine;
    private WaitForSeconds _delay;

    private void Awake()
    {
        _handPattern = GetComponent<DevilHandController>();
        _darkSpearController = GetComponent<DevilDarkSpearController>();
        _delay = new WaitForSeconds(_patternDelay);


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
        Debug.Log("패턴 A");
        // 리턴 있는 플랙홀 자리
        yield return _delay;
        yield return StartCoroutine(_handPattern.CrossPattern());
        yield return _delay;
        StartCoroutine(_handPattern.VerticalPattern());
        StartCoroutine(_darkSpearController.CallDarkSpear());
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
        Debug.Log("패턴 B-1");
        yield return StartCoroutine(_handPattern.CrossPattern());
        yield return _delay;
        yield return StartCoroutine(_handPattern.SpiralPattern());
    }
    IEnumerator PatternB_2()
    {
        Debug.Log("패턴 B-2");
        yield return StartCoroutine(_handPattern.VerticalPattern());
        yield return _delay;
        yield return StartCoroutine(_handPattern.SpiralPattern());
    }
    IEnumerator PatternC()
    {
        Debug.Log("패턴 C");
        StartCoroutine(_darkSpearController.CallDarkSpear());
        //블랙홀 자리
        yield break;
    }
}
