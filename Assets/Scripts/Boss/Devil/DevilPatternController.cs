using System.Collections;
using UnityEngine;

public class DevilPatternController : MonoBehaviour
{
    private DevilHandController _handPattern;
    private DevilDarkSpearController _darkSpearController;
    private DevilBlackHoleController _blackHoleController;

    private Stage3DevilBoss _boss;
    private Boss3DevilData _data;

    private Coroutine _patternLoopCoroutine;
    private WaitForSeconds _delay;

    public void Initialize(Stage3DevilBoss boss)
    {
        _boss = boss;
        _data = boss != null ? boss.Data : null;

        float delay = 1.0f;
        if (_data != null)
        {
            delay = _data.PatternDelay;
        }

        _delay = new WaitForSeconds(delay);
    }

    private void Awake()
    {
        _handPattern = GetComponent<DevilHandController>();
        _darkSpearController = GetComponent<DevilDarkSpearController>();
        _blackHoleController = GetComponent<DevilBlackHoleController>();

        if (_boss == null)
        {
            _boss = GetComponent<Stage3DevilBoss>();
            if (_boss != null)
            {
                Initialize(_boss);
            }
        }
    }

    public void StartPattern()
    {
        StopPattern();
        _patternLoopCoroutine = StartCoroutine(ProcessPatternLoop());
    }

    public void StopPattern()
    {
        if (_patternLoopCoroutine != null) StopCoroutine(_patternLoopCoroutine);
        StopAllCoroutines();
    }

    private IEnumerator ProcessPatternLoop()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    yield return StartCoroutine(PatternA());
                    break;
                case 1:
                    yield return StartCoroutine(PatternB());
                    break;
                case 2:
                    yield return StartCoroutine(PatternC());
                    break;
            }

            yield return _delay;
        }
    }

    IEnumerator PatternA()
    {
        Debug.Log("패턴 A");
        yield return StartCoroutine(_blackHoleController.BlackHoleCoroutine());
        yield return _delay;
        yield return StartCoroutine(_handPattern.CrossPattern());
        yield return _delay;

        Coroutine hand = StartCoroutine(_handPattern.VerticalPattern());
        Coroutine spear = StartCoroutine(_darkSpearController.CallDarkSpear());

        yield return hand;
    }

    IEnumerator PatternB()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                Debug.Log("패턴 B-1");
                yield return StartCoroutine(_handPattern.CrossPattern());
                yield return _delay;
                yield return StartCoroutine(_handPattern.SpiralPattern());
                break;
            case 1:
                Debug.Log("패턴 B-2");
                yield return StartCoroutine(_handPattern.VerticalPattern());
                yield return _delay;
                yield return StartCoroutine(_handPattern.SpiralPattern());
                break;
        }
    }

    IEnumerator PatternC()
    {
        Debug.Log("패턴 C");
        Coroutine bh = StartCoroutine(_blackHoleController.BlackHoleCoroutine());
        Coroutine spear = StartCoroutine(_darkSpearController.CallDarkSpear());

        yield return bh;
    }
}