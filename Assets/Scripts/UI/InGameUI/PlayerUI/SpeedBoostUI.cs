using System.Collections;
using UnityEngine;

public class SpeedBoostUI : MonoBehaviour
{
    [SerializeField] float _blinkSpeed = 5f;

    Player _player;
    SpriteRenderer _iconSprite;

    private void Awake()
    {
        _iconSprite = GetComponent<SpriteRenderer>();
        _iconSprite.enabled = false;
    }

    public void Initialize(Player player)
    {
        _player = player;
    }

    public void StartBoostIcon()
    {
        _iconSprite.enabled = true;
        _iconSprite.color = new Color(1f, 1f, 1f, 0.5f);
        StopAllCoroutines();
        StartCoroutine(BoostIconCoroutine());
    }
    public void StopBoostIcon()
    {
        StopAllCoroutines();
        _iconSprite.enabled = false;
        _iconSprite.color = Color.white;
    }
    private IEnumerator BoostIconCoroutine()
    {
        while (true)
        {
            float alpha = Mathf.Lerp(0.1f, 0.5f, Mathf.PingPong(Time.time * _blinkSpeed, 1f));
            _iconSprite.color = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }
    }
}
