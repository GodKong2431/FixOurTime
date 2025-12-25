using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    private void Start()
    {
        // 애니메이터에서 현재 재생되는 클립의 길이를 가져와서 그 시간 뒤에 파괴
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float length = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, length);
        }
        else
        {
            // 애니메이터가 없으면 1초 뒤 파괴 
            Destroy(gameObject, 1f);
        }
    }
}