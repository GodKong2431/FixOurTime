using UnityEngine;

// 속박(이동/행동 불가) 상태를 적용받을 수 있는 오브젝트가 상속받는 인터페이스
public interface IBindable
{
    // 속박을 적용하는 메서드 (duration: 지속 시간)
    void SetBind(float duration);
}