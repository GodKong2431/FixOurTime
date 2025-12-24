using System.Collections;
using UnityEngine;

public class EnablePanel : MonoBehaviour
{
    [SerializeField] float _duration = 5f;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(DisableAfterDuration());
    }
    IEnumerator DisableAfterDuration()
    {
        yield return new WaitForSeconds(_duration);

        gameObject.SetActive(false);
    }
}
