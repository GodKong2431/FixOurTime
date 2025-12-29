using System.Collections;
using UnityEngine;

public class Steam : DamageableTrapBase
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public IEnumerator OffSteam()
    {
        _animator.SetTrigger("Off");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
