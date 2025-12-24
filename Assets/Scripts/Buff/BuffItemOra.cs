using UnityEngine;

public class BuffItemOra : MonoBehaviour
{
    [Header("깜박임 설정")]
    [SerializeField] float _flickerSpeed = 2f;
    [SerializeField] float _minAlpha = 0.2f;
    [SerializeField] float _maxAlpha = 1f;

    [Header("회전 설정")]
    [SerializeField] float _rotationSpeed = 30f;

    SpriteRenderer _spr;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.Rotate(0,0,_rotationSpeed*Time.deltaTime);

        float alphaRange = _maxAlpha - _minAlpha;
        float currentAlpha = _minAlpha + Mathf.PingPong(Time.time * _flickerSpeed, alphaRange);

        Color c = _spr.color;
        c.a = currentAlpha;
        _spr.color = c;
    }
}
