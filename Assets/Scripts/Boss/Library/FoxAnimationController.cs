using UnityEngine;

public class FoxAnimationController : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _renderer;

    private static readonly int IsShadow = Animator.StringToHash("IsShadow");
    private static readonly int OnJump = Animator.StringToHash("OnJump");
    private static readonly int OnBite = Animator.StringToHash("OnBite");
    private static readonly int IsEating = Animator.StringToHash("IsEating");

    private static readonly int OnShadowExplosion = Animator.StringToHash("OnShadowExplosion");
    private static readonly int OnReset = Animator.StringToHash("OnReset");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetShadowMode(bool isShadow)
    {
        _animator.SetBool(IsShadow, isShadow);
    }

    // ¸Ô±â »óÅÂ ¼³Á¤ (True¸é °è¼Ó ¸Ô´Â ¸ð¼Ç, False¸é ¸ØÃã)
    public void SetEating(bool isEating)
    {
        _animator.SetBool(IsEating, isEating);
    }

    public void TriggerJump() => _animator.SetTrigger(OnJump);
    public void TriggerBite() => _animator.SetTrigger(OnBite);
    public void TriggerShadowExplosion() => _animator.SetTrigger(OnShadowExplosion);

    public void ResetTriggers()
    {
        _animator.SetTrigger(OnReset);
        _animator.SetBool(IsEating, false);
    }

    public void Flip(bool isLeft)
    {
        if (_renderer != null) _renderer.flipX = isLeft;
    }
}