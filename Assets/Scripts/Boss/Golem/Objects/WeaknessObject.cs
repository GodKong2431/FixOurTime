using UnityEngine;

// 피격 발생하기 때문에 IDamageable 상속
public class WeaknessObject : MonoBehaviour, IDamageable
{
    [SerializeField]
    private BossBase _bossBase;

    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider2D _collider;

    private void Awake()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_collider == null) _collider = GetComponent<Collider2D>();
    }

    // 1. 대기 상태 - 이미지 정지, 피격 불가
    public void SetIdleState()
    {
        gameObject.SetActive(true); // 항상 켜둠
        _collider.enabled = false;  // 때릴 수 없음
        _animator.SetBool("IsExposed", false); 
    }

    // 2. 약점 노출 상태 - 애니메이션 재생, 피격 가능
    public void SetExposedState()
    {
        gameObject.SetActive(true);
        _collider.enabled = true;   // 때릴 수 있음
        _animator.SetBool("IsExposed", true); // 활성화 애니메이션
    }

    // 플레이어의 공격이 IDamageable을 찾아서 호출함
    public void TakeDamage(float damage, float knockback, Vector3 hitPos)
    {
        // 실제 HP 깎는 건 컨트롤러에게 위임
        if (_bossBase != null)
        {
           
            _bossBase.TakeDamage(damage);


            Debug.Log($"<color=red>[약점 격파]</color> 보스 남은 체력: {_bossBase.CurrentHp}");
        }

        // 피격 애니메이션 트리거 발동
        _animator.SetTrigger("OnHit");
        _animator.SetBool("IsExposed", false);

        // 중복 피격 방지를 위해 즉시 콜라이더 비활성화
        _collider.enabled = false;
    }
}