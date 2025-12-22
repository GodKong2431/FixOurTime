using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("넉백수치")]
    [SerializeField] private float _knockbackForce = 5f;

    [Header("반사각")]
    [SerializeField] private float _angle = 1f;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //플레이어라면
        if (collision.gameObject.CompareTag("Player"))
        {
            //충돌시점플레이어 위치와 리짓바디 가져오기
            Vector2 playerPos = collision.transform.position;
            Rigidbody2D playerRid = collision.gameObject.GetComponent<Rigidbody2D>();

            //힘을 줄 방향설정
            Vector2 dir = (playerPos - (Vector2)transform.position).normalized;

            dir.y = _angle;
            


            //기존속도 초기화
            playerRid.linearVelocity = Vector2.zero;
            playerRid.AddForce(dir * _knockbackForce, ForceMode2D.Impulse);
        }
    }
}
