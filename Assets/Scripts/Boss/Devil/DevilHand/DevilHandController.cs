using System.Collections;
using UnityEngine;

public class DevilHandController : MonoBehaviour
{
    [SerializeField] private DevilHand _leftHand;
    [SerializeField] private DevilHand _rightHand;

    [Header("11자 시작 오프셋 (보스 기준)")]
    [SerializeField] private Vector2 _verticalStartOffset;
    [SerializeField] private float _verticalReadyTime = 0.4f;
    [SerializeField] private float _verticalAfterAttackTime = 0.6f;

    [Header("X자 시작 오프셋 (보스 기준)")]
    [SerializeField] private Vector2 _crossStartOffset;
    [SerializeField] private float _crossReadyTime = 0.4f;
    [SerializeField] private float _crossAfterAttackTime = 0.6f;

    [Header("회전 시작 오프셋 (보스 기준)")]
    [SerializeField] private Vector2 _spiralStartOffsetX;
    [SerializeField] private float _spiralReadyTime = 0.4f;
    [SerializeField] private float _spiralAfterAttackTime = 0.6f;
    [SerializeField] private float _spiraloffset = 6f;
    [SerializeField] private float _spiralDuration = 3f;

    private Boss3DevilData _data;

    private void Awake()
    {
        var boss = GetComponentInParent<Stage3DevilBoss>();
        if (boss != null)
        {
            _data = boss.Data;

            if (_data != null)
            {
                _leftHand.Configure(_data.HandMoveSpeed, _data.HandAttackSpeed, _data.HandDamage);
                _rightHand.Configure(_data.HandMoveSpeed, _data.HandAttackSpeed, _data.HandDamage);

                _verticalReadyTime = _data.VerticalReadyTime;
                _verticalAfterAttackTime = _data.VerticalAfterAttackTime;
                _crossReadyTime = _data.CrossReadyTime;
                _crossAfterAttackTime = _data.CrossAfterAttackTime;
                _spiralReadyTime = _data.SpiralReadyTime;
                _spiralAfterAttackTime = _data.SpiralAfterAttackTime;
                _spiraloffset = _data.SpiralOffset;
                _spiralDuration = _data.SpiralDuration;
            }
        }
    }

    /// <summary>
    /// 11자 내려찍기 패턴
    /// </summary>
    /// <returns></returns>
    public IEnumerator VerticalPattern()
    {
        _leftHand.BeginPattern(new Vector2(-_verticalStartOffset.x, _verticalStartOffset.y));
        _rightHand.BeginPattern(_verticalStartOffset);
        yield return new WaitForSeconds(_verticalReadyTime);

        // 시작 위치로 이동 완료할 때까지 대기
        yield return new WaitUntil(() => !_leftHand.IsBusy && !_rightHand.IsBusy);

        yield return new WaitForSeconds(_verticalReadyTime);

        _leftHand.Attack(Vector2.down);
        _rightHand.Attack(Vector2.down);

        // 손이 바닥에 닿을 때까지(IsBusy가 false될 때까지) 대기
        yield return new WaitUntil(() => !_leftHand.IsBusy && !_rightHand.IsBusy);

        yield return new WaitForSeconds(_verticalReadyTime);

        _leftHand.MoveToReturnPos();
        _rightHand.MoveToReturnPos();

        // 완전히 복귀할 때까지 대기 (그래야 다음 패턴이 안 꼬임)
        yield return new WaitUntil(() => !_leftHand.IsBusy && !_rightHand.IsBusy);
    }

    /// <summary>
    /// X자 내려찍기 패턴
    /// </summary>
    /// <returns></returns>
    public IEnumerator CrossPattern()
    {
        _leftHand.BeginPattern(new Vector2(-_crossStartOffset.x, _crossStartOffset.y));
        _rightHand.BeginPattern(_crossStartOffset);

        yield return new WaitForSeconds(_crossReadyTime);

        Vector2 down = Vector2.down;
        Vector2 leftDir = Quaternion.Euler(0, 0, 45f) * down;
        Vector2 rightDir = Quaternion.Euler(0, 0, -45f) * down;

        _leftHand.Attack(leftDir);
        _rightHand.Attack(rightDir);

        // 공격 끝날 때까지 대기
        yield return new WaitUntil(() => !_leftHand.IsBusy && !_rightHand.IsBusy);

        yield return new WaitForSeconds(_crossAfterAttackTime);

        _leftHand.MoveToReturnPos();
        _rightHand.MoveToReturnPos();

        // 복귀 대기
        yield return new WaitUntil(() => !_leftHand.IsBusy && !_rightHand.IsBusy);
    }

    /// <summary>
    /// 스파이럴 패턴
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpiralPattern()
    {
        _leftHand.BeginPattern(new Vector2(-_spiralStartOffsetX.x, _spiralStartOffsetX.y - 10));
        _rightHand.BeginPattern(new Vector2(_spiralStartOffsetX.x, -_spiralStartOffsetX.y - 10));

        yield return new WaitUntil(() => !_leftHand.IsBusy && !_rightHand.IsBusy);

        yield return new WaitForSeconds(_spiralReadyTime);

        Vector2 center = ((Vector2)_leftHand.transform.position + (Vector2)_rightHand.transform.position) * 0.5f;

        _leftHand.SpiralAttack(center, _spiraloffset, _spiralDuration);
        _rightHand.SpiralAttack(center, _spiraloffset, _spiralDuration);

        // 회전 끝날 때까지 대기 (SpiralAttack은 내부적으로 duration만큼 돔)
        yield return new WaitUntil(() => !_leftHand.IsBusy && !_rightHand.IsBusy);

        yield return new WaitForSeconds(_spiralAfterAttackTime);

        _leftHand.MoveToReturnPos();
        _rightHand.MoveToReturnPos();

        // 복귀 대기
        yield return new WaitUntil(() => !_leftHand.IsBusy && !_rightHand.IsBusy);
    }

    public void ForceResetHands()
    {
        StopAllCoroutines();
        if (_leftHand != null) _leftHand.ForceReturn();
        if (_rightHand != null) _rightHand.ForceReturn();
    }
}