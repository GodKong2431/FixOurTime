using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public enum ItemType { SecondHand,MinuteHand,HourHand}
    [Header("아이템 설정")]
    [SerializeField] ItemType _itemType;
    [SerializeField] float _buffDuration = 99999f;

    Player _player;

    private void Start()
    {
        _player = GameManager.Instance.Player;

        OnPlayerRespawnCheck();
    }
    public void OnPlayerRespawnCheck()
    {
        GameData data = GameDataManager.Load();

        if (_player == null) return;

        bool isCollected = false;
        switch (_itemType)
        {
            case ItemType.SecondHand: isCollected = _player.HasSecondHand; break;
            case ItemType.MinuteHand: isCollected = _player.HasMinuteHand; break;
            case ItemType.HourHand: isCollected = _player.HasHourHand; break;
        }

        if (isCollected)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(_player);
            _player.CollectItem(_itemType);//이 아이템 수집했다 플래그 갱신용

            SoundManager.Instance.PlaySFX("SFX_ClockHand_Get");

            gameObject.SetActive(false); 
        }
    }
    private void ApplyEffect(Player player)
    {
        switch (_itemType)
        {
            case ItemType.SecondHand:
                player.AddEffect(new SecondHandBuff(_buffDuration));
                Debug.Log("초침 획득");
                break;

            case ItemType.MinuteHand:
                player.AddEffect(new MinuteHandBuff(_buffDuration));
                Debug.Log("분침 획득");
                break;

            case ItemType.HourHand:
                player.AddEffect(new HourHandBuff(_buffDuration));
                Debug.Log("시침 획득");
                break;
        }
    }
}
