using UnityEngine;

// 피격 발생하기 때문에 IDamageable 상속
public class WeaknessObject : MonoBehaviour, IDamageable
{
    [SerializeField]
    private BossBase _bossBase;

    // 플레이어의 공격이 IDamageable을 찾아서 호출함
    public void TakeDamage(float damage, float knockback, Vector3 hitPos)
    {
        // 실제 HP 깎는 건 컨트롤러에게 위임
        if (_bossBase != null)
        {
           
            _bossBase.TakeDamage(damage);


            Debug.Log($"<color=red>[약점 격파]</color> 보스 남은 체력: {_bossBase.CurrentHp}");
        }

        // 맞았으니 약점 패턴 종료를 위해 비활성화
        gameObject.SetActive(false);
    }
}