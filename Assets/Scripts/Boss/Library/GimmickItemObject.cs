using UnityEngine;
using System.Collections;

public class GimmickItemObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    // 이 아이템이 정답(Target)인지 여부
    public bool IsTarget { get; private set; }

    private Stage2Boss _boss;
    private bool _canInteract = false;

    public void Initialize(Stage2Boss boss, Sprite sprite, bool isTarget)
    {
        _boss = boss;
        IsTarget = isTarget;

        _renderer.sprite = sprite;
        _renderer.enabled = true;
        _canInteract = true;

        // 2초 뒤 투명화 -> 테스트를 위해 주석 처리
        // StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(_boss.Data.ItemHideDelay);
        _renderer.enabled = false; // 안 보이게 (충돌은 유지)
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_canInteract) return;

        if (collision.CompareTag("Player"))
        {
            _canInteract = false;

            // 플레이어가 먹음 -> 보스에게 결과 보고
            _boss.OnPlayerCollectItem(IsTarget, _renderer.sprite);

            // 먹으면 즉시 사라짐
            Destroy(gameObject);
        }
    }
}