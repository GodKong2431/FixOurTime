using UnityEngine;

// 속박(이동/행동 불가) 상태를 적용받을 수 있는 오브젝트가 상속받는 인터페이스
public interface IBindable
{
    void Bind(bool Isbind); // 묶임 처리
    void Unbind(bool Isbind);               // 풀림 처리

    void SetBind(float duration); // 일정 시간 동안 묶임 처리
}
