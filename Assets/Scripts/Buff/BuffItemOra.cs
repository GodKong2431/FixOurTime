using UnityEngine;

public class BuffItemOra : MonoBehaviour
{
    [Header("깜박임 설정")]
    [SerializeField] float _flickerSpeed = 2f;
    [SerializeField] float _minAlpha = 0.2f;
    [SerializeField] float _maxAlpha = 1f;

    [Header("회전 설정")]
    [SerializeField] float _rotationSpeed = 30f;

    [Header("부유 설정")]
    [SerializeField] float _floatingDistance = 0.2f;  //위아래 움직이는 거리
    [SerializeField] float _floatingSpeed = 2f;

    SpriteRenderer _spr;
    Vector3 _startPos;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
        _startPos = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0,0,_rotationSpeed*Time.deltaTime);

        float alphaRange = _maxAlpha - _minAlpha;
        float currentAlpha = _minAlpha + Mathf.PingPong(Time.time * _flickerSpeed, alphaRange);

        Color c = _spr.color;
        c.a = currentAlpha;
        _spr.color = c;

        float floating = _startPos.y + Mathf.Sin(Time.time * _floatingSpeed) * _floatingDistance;
        transform.parent.position = new Vector3(_startPos.x, floating, _startPos.z);
    }
}
