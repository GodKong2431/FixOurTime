using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    SpriteRenderer _spr;
    
    float _damage;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
        
    }

    public void Activate(float damage, Vector2 size)
    {
        _damage = damage;
        transform.localScale = size;
        if (_spr != null)
        {
            Color c = _spr.color;
            c.a = 1f;
            _spr.color = c;
        }
        gameObject.SetActive(true);
    }

    public void UpdateEffect(float progress)
    {
        if (_spr == null)
        {
            return;
        }

        if(progress >= 0.5f)
        {
            float alpha = 1f - ((progress - 0.5f) * 2f);
            Color c = _spr.color;
            c.a = Mathf.Clamp01(alpha);
            _spr.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();
        if(target != null)
        {
            target.TakeDamage(_damage, 5f, transform.position);

            //흔들림넣을거면 매니저호출
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
