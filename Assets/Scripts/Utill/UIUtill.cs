using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class UIUtill
{
    //여러이미지들 페이드인아웃 반복효과
    public static IEnumerator FadeRoutine(Image current, Image next, float _fadeDuration)
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

        //안보이는 이미지 꺼두기
        current.gameObject.SetActive(false);
    }


    //UI 아래에서 위로 올리는 효과

    public static IEnumerator UpMoving(RectTransform target, float offsetY, float duration)
    {
        //목적지
        Vector2 endPos = target.anchoredPosition;
        //출발지점 (목적지에서 오프셋만큼 떨어진곳)
        Vector2 startPos = endPos - new Vector2(0, offsetY);

        float timer = 0f;

        //시작위치로 이동
        target.anchoredPosition = startPos;

        //지속시간만큼 반복
        while (timer < duration)
        {
            timer += Time.deltaTime;
            //스무스스탭적용변수 (0 ~ 1까지 부드러운이동)
            float currentTime = Mathf.SmoothStep(0f, 1f, timer / duration);
            //부드럽게 이동
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, currentTime);
            //한프레임 쉬게하기
            yield return null;
        }
        //마지막 위치 고정
        target.anchoredPosition = endPos;
    }
}
