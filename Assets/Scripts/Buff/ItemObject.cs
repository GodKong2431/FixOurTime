using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public enum ItemType { SecondHand,MinuteHand,HourHand}
    [Header("¾ÆÀÌÅÛ ¼³Á¤")]
    [SerializeField] ItemType _itemType;
    [SerializeField] float _buffDuration = 99999f;

    Player _player;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
    public void OnPlayerRespawnCheck()
    {

    }
    private void CheckAlreadyCollected()
    {
        if (_player == null) return;

        bool isCollected = false;
        switch (_itemType)
        {
            case ItemType.SecondHand: isCollected = _player.HasSecondHand;break;
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
            _player.CollectItem(_itemType);//ÀÌ ¾ÆÀÌÅÛ ¼öÁýÇß´Ù ÇÃ·¡±× °»½Å¿ë
            gameObject.SetActive(false); 

            //ºØ±«¼ÓµµÁ¶Àý
            if (_itemType == ItemType.HourHand)
            {
                //ºØ±«¼Óµµ 2¹è Áõ°¡
            }

            

            //ºØ±«±¸¿ª¿¡ ¾ÆÀÌÅÛ ´êÀ¸¸é
            //if (other.CompareTag("ºØ±«±¸¿ª"))
            //{
            //    //°ÔÀÓ¿À¹ö ¼³Á¤
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
                Debug.Log("ÃÊÄ§ È¹µæ");
                break;

            case ItemType.MinuteHand:
                player.AddEffect(new MinuteHandBuff(_buffDuration));
                Debug.Log("ºÐÄ§ È¹µæ");
                break;

            case ItemType.HourHand:
                player.AddEffect(new HourHandBuff(_buffDuration));
                Debug.Log("½ÃÄ§ È¹µæ");
                break;
        }
    }
}
