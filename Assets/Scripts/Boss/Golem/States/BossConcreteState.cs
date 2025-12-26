using UnityEngine;
using System.Collections;

public class BossConcreteState : BossState
{
    private int _count;
    private bool _isRetract; // 패턴이 끝나고 회수 할 지 여부

    // 생성자 2: 횟수와 회수 여부를 받음
    public BossConcreteState(Stage1Boss _boss, int count, bool doRetract) : base(_boss)
    {
        _count = count;
        _isRetract = doRetract;
    }

    public override void Enter() { }
    public override void Exit()
    {
        Stage1Boss boss = _baseBoss as Stage1Boss;
        if (boss != null)
        {
            if (boss.WallFistObject) boss.WallFistObject.gameObject.SetActive(false);
            if (boss.FloorFistObject) boss.FloorFistObject.gameObject.SetActive(false);
        }
    }

    public override IEnumerator Execute()
    {

        Stage1Boss _boss = _baseBoss as Stage1Boss;
        if (_boss == null) yield break;

        for (int i = 0; i < _count; i++)
        {
            bool isHorizontal = (i % 2 == 0);

            // 가로면 벽 주먹, 세로면 바닥 주먹 선택
            Transform currentFist = isHorizontal ? _boss.WallFistObject : _boss.FloorFistObject;
            currentFist.gameObject.SetActive(true);

            Vector3 startPos = currentFist.position; // 초기 위치
            Vector3 targetPos;
            float moveDist = _boss.BossData.BossAppearDistance;

            // 1. 등장 위치 설정 (플레이어 추적)
            if (isHorizontal)
            {
                // 벽 주먹: Y축 추적
                Vector3 currentPos = currentFist.position;
                currentPos.y = _boss.PlayerTarget.position.y;
                currentFist.position = currentPos;

                startPos = currentPos; // 시작점 갱신
                targetPos = startPos + Vector3.left * moveDist;
            }
            else
            {
                // 바닥 주먹: X축 추적
                Vector3 currentPos = currentFist.position;
                currentPos.x = _boss.PlayerTarget.position.x;
                currentFist.position = currentPos;

                startPos = currentPos; // 시작점 갱신
                targetPos = startPos + Vector3.up * (moveDist - 2.0f); // 높이 보정
            }

            // 2. 펀치 액션

            // (1) 예비 동작 위치 계산 (목표 거리의 20%만 살짝 나옴)
            Vector3 telegraphPos = Vector3.Lerp(startPos, targetPos, 0.4f);

            // (2) 살짝 튀어나옴
            yield return _boss.StartCoroutine(_boss.MoveBossTo(currentFist, telegraphPos, 0.2f));

            // (3) 공격 전 딜레이
            yield return new WaitForSeconds(0.5f);

            float punchSpeed = _boss.BossData.BossMoveDuration * 0.2f;
            yield return _boss.StartCoroutine(_boss.MoveBossTo(currentFist, targetPos, punchSpeed));

            // 3. 콘크리트 생성 위치 계산
            Vector3 spawnOffset = isHorizontal
                ? Vector3.left * _boss.BossData.ConcreteSpawnOffsetH
                : Vector3.up * _boss.BossData.ConcreteSpawnOffsetV;

            Vector3 finalSpawnPos = currentFist.position + spawnOffset;

            // 4. 콘크리트 생성
            GameObject prefab = isHorizontal ? _boss.ConcreteHPrefab : _boss.ConcreteVPrefab;
            Quaternion rotation = isHorizontal ? Quaternion.identity : Quaternion.Euler(0, 0, -90f);

            GameObject concreteObj = Object.Instantiate(prefab, finalSpawnPos, rotation);
            ConcreteObject concreteScript = concreteObj.GetComponent<ConcreteObject>();

            concreteScript.Initialize(isHorizontal, _boss.CenterPoint.position, _boss.BossData);
            _boss.RegisterConcrete(concreteScript);

            yield return new WaitForSeconds(_boss.BossData.ConcreteInterval);

            // 5. 주먹 회수
            yield return _boss.StartCoroutine(_boss.MoveBossTo(currentFist, startPos, _boss.BossData.BossMoveDuration));
            currentFist.gameObject.SetActive(false); // 사용 후 숨김

            // 다음 패턴 대기
            yield return new WaitForSeconds(_boss.BossData.PatternWaitTime);
        }

        if (_isRetract)
        {
            _boss.RetractAllConcretes();
        }
    }
}