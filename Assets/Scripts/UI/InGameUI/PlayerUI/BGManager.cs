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
            yield return StartCoroutine(FadeRoutine(_image[currentIndex], _image[nextIndex]));

            //교체 완료되면 인덱스 업데이트
            currentIndex = nextIndex;
        }
    }

    IEnumerator FadeRoutine(Image current, Image next)
    {
        float timer = 0;    

        next.gameObject.SetActive(true); // 다음 이미지 활성화

        Color currentColor = current.color;
        Color nextColor = next.color;

        nextColor.a = 0f;   //처음에는 투명

        while (timer < _fadeDuration)   
        {
            timer += Time.deltaTime;
            float progress = timer / _fadeDuration;

            currentColor.a = 1 - progress; // 현재이미지 점점 투명하게
            nextColor.a = progress;       // 다음 이미지 점점 불투명하게

            //변경된 투명도 컴포넌트에 적용
            current.color = currentColor;
            next.color = nextColor;

            yield return null;
        }
        //페이드 끝난후 값 고정
        currentColor.a = 0;
        nextColor.a = 1;
    }
}
