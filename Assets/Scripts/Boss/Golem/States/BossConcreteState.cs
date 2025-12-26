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
    public override void Exit() { }

    public override IEnumerator Execute()
    {

        Stage1Boss _boss = _baseBoss as Stage1Boss;
        if (_boss == null) yield break;

        for (int i = 0; i < _count; i++)
        {
            bool isHorizontal = (i % 2 == 0);
            Transform currentBoss = isHorizontal ? _boss.WallBossObject : _boss.FloorBossObject;

            Vector3 startPos = currentBoss.position;
            Vector3 targetPos;


            float moveDist = _boss.BossData.BossAppearDistance;

            // 보스 등장 위치 설정
            if (isHorizontal)
            {
                startPos.y = _boss.PlayerTarget.position.y;
                currentBoss.position = startPos;
                targetPos = startPos + Vector3.left * moveDist;
            }
            else
            {
                startPos.x = _boss.PlayerTarget.position.x;
                currentBoss.position = startPos;
                targetPos = startPos + Vector3.up * (moveDist -2.0f);
            }

            // 보스 등장 이동
            yield return _boss.StartCoroutine(_boss.MoveBossTo(currentBoss, targetPos, _boss.BossData.BossMoveDuration));
            
            // 공격 전 대기
            yield return new WaitForSeconds(_boss.BossData.PatternWaitTime);

            // 2. 콘크리트 생성 위치 계산 
            Vector3 spawnOffset = Vector3.zero;


            if (isHorizontal)
            {
                spawnOffset = Vector3.left * _boss.BossData.ConcreteSpawnOffsetH;
            }
            else
            {
                spawnOffset = Vector3.up * _boss.BossData.ConcreteSpawnOffsetV;
            }

            Vector3 finalSpawnPos = currentBoss.position + spawnOffset;

            // 3. 콘크리트 생성 및 초기화
            GameObject prefab = isHorizontal ? _boss.ConcreteHPrefab : _boss.ConcreteVPrefab;

            Quaternion rotation = Quaternion.identity;
            if (!isHorizontal)
            {
                rotation = Quaternion.Euler(0, 0, -90f);
            }

            GameObject concreteObj = Object.Instantiate(prefab, finalSpawnPos, rotation);

            //스크립트 가져오기
            ConcreteObject concreteScript = concreteObj.GetComponent<ConcreteObject>();

            // 데이터 전달 (속도, 유지시간 등)
            concreteObj.GetComponent<ConcreteObject>().Initialize(
                isHorizontal,
                _boss.CenterPoint.position,
                _boss.BossData
            );

            // 컨트롤러 명단에 등록
            _boss.RegisterConcrete(concreteScript);

            yield return new WaitForSeconds(_boss.BossData.ConcreteInterval);

            yield return _boss.StartCoroutine(_boss.MoveBossTo(currentBoss, startPos, _boss.BossData.BossMoveDuration));

            // 다음 패턴까지 대기
            yield return new WaitForSeconds(_boss.BossData.PatternWaitTime);
        }

        // 모든 생성이 끝난 후, 회수 옵션이 켜져있으면 일괄 회수
        if (_isRetract)
        {
            _boss.RetractAllConcretes();
        }
    }
}