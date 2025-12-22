using System.Collections.Generic;
using UnityEngine;

public class DayZone : MonoBehaviour
{
    private Player _player;
    private bool _isPlayerInside = false;
    public float _stayTimer = 0f;

    private void Update()
    {
        // 플레이어가 구역 안에 있을 때만 실행
        if (_isPlayerInside && _player != null)
        {
            // 밤 디버프 즉시 제거
            _player.RemoveEffectByName("추움");

            _stayTimer += Time.deltaTime;

            if (_stayTimer >= 5f)
            {
                // 5초 경과 시 탈진 (나간 뒤에도 유지되도록 Duration을 길게 설정)
                _player.RemoveEffectByName("더움");
                _player.AddEffect(new ExhaustDebuff(9999f));
            }
            else
            {
                // 5초 전에는 더움 (나간 뒤 5초 유지)
                _player.AddEffect(new HotDebuff(5f));
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _player = collision.GetComponent<Player>();
            _isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInside = false;
            _stayTimer = 0f; // 낮 구역 체류 시간 초기화

            // 탈진은 여기서 지우지 않음 -> 밤 구역이나 분수대에서 지움
        }
    }
}
