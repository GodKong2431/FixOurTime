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

                Debug.Log("체크포인트 저장");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isSaved) return;

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            if (player != null && !_isSaved)
            {
                player.CheckPoint(transform.position);
                _isSaved = true;

                Debug.Log("체크포인트 저장");
            }
        }
    }
}
