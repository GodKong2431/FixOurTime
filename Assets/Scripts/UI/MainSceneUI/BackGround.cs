using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackGround : MonoBehaviour
{
    [Header("사용할 이미지 및 순서")]
    [SerializeField] private Image[] _image;

    [Header("페이드 시간")]
    [SerializeField] private float _fadeDuration;

    [Header("이미지유지 시간")]
    [SerializeField] private float _displayDuration;

    private int currentIndex = 0;   //인덱스 체크용

    void Start()
    {
        //첫번째 배경만 켜고 나머지는 꺼두기
        for (int i = 0; i < _image.Length; i++)
        {
            //컬러값가져오기
            Color color = _image[i].color;

            //i = 0이면 투명도 켜두기
            color.a = (i == 0) ? 1 : 0;

            //컬러적용
            _image[i].color = color;

            //i = 0이면 활성화
            _image[i].gameObject.SetActive(i == 0);
        }

        
        StartCoroutine(BackgroundCycleRoutine());
    }

    
    //무한루프 코루틴
    IEnumerator BackgroundCycleRoutine()
    {
        while (true)
        {
            //현재 이미지유지시간 (초기셋팅 첫번째 배경)
            yield return new WaitForSeconds(_displayDuration);

            //유지시간 끝나면 다음으로 넘어갈 인덱스 계산
            //4번째 이미지마다 다시 첫번째 이미지로 가야되니깐 나머지 연산
            int nextIndex = (currentIndex + 1) % _image.Length;

            //계산한 인덱스 값으로 페이드코루틴 실행
            yield return StartCoroutine(FadeRoutine(_image[currentIndex], _image[nextIndex]));

            //다음 인덱스 대입
            currentIndex = nextIndex;
        }
    }

    IEnumerator FadeRoutine(Image current, Image next)
    {
        float timer = 0;    //내부에서만 쓰일 페이드 코루틴용 타이머

        next.gameObject.SetActive(true); // 다음 이미지 활성화

        //컬러값 가져오기
        Color currentColor = current.color;
        Color nextColor = next.color;

        nextColor.a = 0f;   //다음페이지는 알파값 0으로 셋팅

        while (timer < _fadeDuration)   //페이드시간만큼 반복
        {
            timer += Time.deltaTime;
            float progress = timer / _fadeDuration;

            currentColor.a = 1 - progress; //페이드아웃
            nextColor.a = progress;       //페이드인

            //컬러적용
            current.color = currentColor;
            next.color = nextColor;

            yield return null;
        }

        currentColor.a = 0;
        nextColor.a = 1;
    }
}