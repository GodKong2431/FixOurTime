using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class SavePoint : MonoBehaviour
{
    SpriteRenderer _spr;
    BoxCollider2D _collider;

    private bool _isSaved = false;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if(player != null && !_isSaved)
            {
                player.CheckPoint(transform.position);
                _isSaved = true;
                _spr.color = Color.green;

                Debug.Log("체크포인트 저장");
            }
        }
    }
}
