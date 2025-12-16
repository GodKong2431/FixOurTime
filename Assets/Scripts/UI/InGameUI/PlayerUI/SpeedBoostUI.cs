using System.Collections;
using UnityEngine;

public class SpeedBoostUI : MonoBehaviour
{
    [SerializeField] float _blinkSpeed = 5f;
    [SerializeField] float _blinkStartRatio = 0.7f;

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
        float duration = _player.BoostDuration;
        float timer = 0f;

        float blinkStartTime = duration * _blinkStartRatio;

        while(timer < duration)
        {
            timer += Time.deltaTime;
            if(timer >= blinkStartTime)
            {
                float alpha = Mathf.Lerp(0.1f, 0.5f, Mathf.PingPong(Time.time * _blinkSpeed, 1f));
                _iconSprite.color = new Color(1f,1f,1f,alpha);
            }
            yield return null;
        }
        StopBoostIcon();
    }
}
