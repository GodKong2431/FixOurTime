using UnityEngine;

public class DestroyArea : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float _riseSpeed = 2f;

    [Header("References")]
    public Transform _destroyLine;

    public float TopY => _destroyLine.position.y;

    void Update()
    {
        transform.Translate(Vector2.up * _riseSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("플레이어 사망");
        }
    }
}
