using NUnit.Framework.Internal.Builders;
using UnityEngine;
using UnityEngine.Pool;

public class PaperCrane : DamageableTrapBase, IDamageable
{
    [Header("¼Óµµ")]
    [SerializeField] private float _Speed;

    PaperCraneSpawner _paperCraneSpawner;
    GameObject _player;

    private void Start()
    {
        _player = GameManager.Instance.Player.gameObject;
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _Speed * Time.deltaTime);
    }

    public void Init(GameObject player, PaperCraneSpawner paperCraneSpawner)
    {
        _player = player;
        _paperCraneSpawner = paperCraneSpawner;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Player player))
        {
            player.TakeDamage(_trapDamage, _knockbackForce, transform.position);
            _paperCraneSpawner.Release(gameObject);
        }
    }

    public void TakeDamage(float damage, float KnockbackForce, Vector3 hitPos)
    {
        _paperCraneSpawner.Release(gameObject);
    }
}
