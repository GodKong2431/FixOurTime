using UnityEngine;
using System.Collections;

// 플레이어 공격에 반응하여 획득되는 아이템
public class GimmickItemObject : MonoBehaviour, IDamageable
{
    [SerializeField] private SpriteRenderer _renderer;

    // 이 아이템이 정답(Target)인지 여부
    public bool IsTarget { get; private set; }

    private Stage2Boss _boss;
    private bool _canInteract = false;

    // 공격 판정을 위한 콜라이더
    [SerializeField] private BoxCollider2D _collider;

    public void Initialize(Stage2Boss boss, Sprite sprite, bool isTarget)
    {
        _boss = boss;
        IsTarget = isTarget;

        _renderer.sprite = sprite;
        _renderer.enabled = true;
        _canInteract = true;

        if (_collider == null) _collider = GetComponent<BoxCollider2D>();
        if (_collider != null) _collider.enabled = true;
    }

    // 플레이어가 공격하면 아이템 획득
    public void TakeDamage(float damage, float knockback, Vector3 hitPos)
    {
        if (!_canInteract) return;
        CollectItem();
    }

    private void CollectItem()
    {
        _canInteract = false;

        // 플레이어가 먹음 -> 보스에게 결과 보고
        _boss.OnPlayerCollectItem(IsTarget, _renderer.sprite);

        // 먹으면 즉시 사라짐
        Destroy(gameObject);
    }
}