using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BGManager : MonoBehaviour
{
    [Header("배경이미지 배열")]
    [SerializeField] private Image[] _image;

    [Header("교체시간(페이드인/아웃)")]
    [SerializeField] private float _fadeDuration;

    [Header("이미지 유지시간")]
    [SerializeField] private float _displayDuration;

    private int currentIndex = 0;   //현재 BG이미지 인덱스

    void Start()
    {
        //모든 배경 이미지 초기상태 설정
        for (int i = 0; i < _image.Length; i++)
        {
            
            Color color = _image[i].color;

            //첫 이미지만 불투명하게 나머지는 투명
            color.a = (i == 0) ? 1 : 0;
            _image[i].color = color;

            //첫 이미지만 활성화 나머지 끄기
            _image[i].gameObject.SetActive(i == 0);
        }

        //배경 순환 루틴
        StartCoroutine(BackgroundCycleRoutine());
    }


    //배경 순환 코루틴
    IEnumerator BackgroundCycleRoutine()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(_displayDuration);

            //다음 보여줄 이미지 번호 계산 , 배열끝 도달하면 다시 0으로
            int nextIndex = (currentIndex + 1) % _image.Length;

            //페이드 시작
            yield return StartCoroutine
                //유틸클래스 페이드아웃 코루틴 호출
                (UIUtill.FadeRoutine(_image[currentIndex], _image[nextIndex], _fadeDuration));

            //교체 완료되면 인덱스 업데이트
            currentIndex = nextIndex;
        }
    }
}
