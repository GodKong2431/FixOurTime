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
    

    private IEnumerator VerticalPattern()
    {
        _leftHand.BeginPattern(new Vector2(-_verticalStartOffset.x, _verticalStartOffset.y));
        _rightHand.BeginPattern(_verticalStartOffset);

        _leftHand.MoveToStartPos();
        _rightHand.MoveToStartPos();

        yield return new WaitForSeconds(_verticalReadyTime);

        _leftHand.Attack(Vector2.down);
        _rightHand.Attack(Vector2.down);

        yield return new WaitForSeconds(_verticalAfterAttackTime);

        _leftHand.MoveToReturnPos();
        _rightHand.MoveToReturnPos();
    }

    private IEnumerator CrossPattern()
    {
        _leftHand.BeginPattern(new Vector2(-_crossStartOffset.x, _crossStartOffset.y));
        _rightHand.BeginPattern(_crossStartOffset);

        _leftHand.MoveToStartPos();
        _rightHand.MoveToStartPos();

        yield return new WaitForSeconds(_crossReadyTime);

        Vector2 down = Vector2.down;
        Vector2 leftDir = Quaternion.Euler(0, 0, 45f) * down;
        Vector2 rightDir = Quaternion.Euler(0, 0, -45f) * down;

        _leftHand.Attack(leftDir);
        _rightHand.Attack(rightDir);

        yield return new WaitForSeconds(_crossAfterAttackTime);

        _leftHand.MoveToReturnPos();
        _rightHand.MoveToReturnPos();
    }

    private IEnumerator SpiralPattern()
    {
        _leftHand.BeginPattern(new Vector2(-_spiralStartOffsetX.x, _spiralStartOffsetX.y - 10));
        _rightHand.BeginPattern(new Vector2(_spiralStartOffsetX.x, -_spiralStartOffsetX.y - 10));

        _leftHand.MoveToStartPos();
        _rightHand.MoveToStartPos();

        yield return new WaitForSeconds(_spiralReadyTime);

        Vector2 center = ((Vector2)_leftHand.transform.position + (Vector2)_rightHand.transform.position) * 0.5f;

        _leftHand.SpiralAttack(center, _spiraloffset, _spiralDuration);
        _rightHand.SpiralAttack(center, _spiraloffset, _spiralDuration);

        yield return new WaitForSeconds(_spiralAfterAttackTime);

        _leftHand.MoveToReturnPos();
        _rightHand.MoveToReturnPos();
    }

    
}
