using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public enum ItemType { SecondHand,MinuteHand,HourHand}
    [Header("아이템 설정")]
    [SerializeField] ItemType _itemType;
    [SerializeField] float _buffDuration = 99999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            ApplyEffect(player);
            player.CollectItem(_itemType); //이 아이템 수집했다 플래그 갱신용

            //붕괴속도조절
            if (_itemType == ItemType.HourHand)
            {
                //붕괴속도 2배 증가
            }

            gameObject.SetActive(false);

            //붕괴구역에 아이템 닿으면
            //if (other.CompareTag("붕괴구역"))
            //{
            //    //게임오버 설정
            //    
            //}
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
