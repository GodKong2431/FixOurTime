using UnityEngine;
using System.Collections;

public class BossConcreteState : BossState
{
    private int _count;
    private bool _doRetract; // 패턴이 끝나고 회수 할 지 여부

    // 생성자 2: 횟수와 회수 여부를 받음
    public BossConcreteState(BossController controller, int count, bool doRetract) : base(controller)
    {
        _count = count;
        _doRetract = doRetract;
    }

    public override void Enter() { }
    public override void Exit() { }

    public override IEnumerator Execute()
    {
        for (int i = 0; i < _count; i++)
        {
            bool isHorizontal = (i % 2 == 0);
            Transform currentBoss = isHorizontal ? _controller.wallBossObject : _controller.floorBossObject;

            Vector3 startPos = currentBoss.position;
            Vector3 targetPos;


            float moveDist = _controller.Data.bossAppearDistance;

            // 보스 등장 위치 설정
            if (isHorizontal)
            {
                startPos.y = _controller.player.position.y;
                currentBoss.position = startPos;
                targetPos = startPos + Vector3.left * moveDist;
            }
            else
            {
                startPos.x = _controller.player.position.x;
                currentBoss.position = startPos;
                targetPos = startPos + Vector3.up * (moveDist -2.0f);
            }

            // 보스 등장 이동
            yield return _controller.StartCoroutine(_controller.MoveBossTo(currentBoss, targetPos, _controller.Data.bossMoveDuration));
            
            // 공격 전 대기
            yield return new WaitForSeconds(_controller.Data.patternWaitTime);

            // 2. 콘크리트 생성 위치 계산 
            Vector3 spawnOffset = Vector3.zero;


            if (isHorizontal)
            {
                spawnOffset = Vector3.left * _controller.Data.concreteSpawnOffsetH;
            }
            else
            {
                spawnOffset = Vector3.up * _controller.Data.concreteSpawnOffsetV;
            }

            Vector3 finalSpawnPos = currentBoss.position + spawnOffset;

            // 3. 콘크리트 생성 및 초기화
            GameObject prefab = isHorizontal ? _controller.concreteHPrefab : _controller.concreteVPrefab;
            GameObject concreteObj = Object.Instantiate(prefab, finalSpawnPos, Quaternion.identity);

            //스크립트 가져오기
            ConcreteObject concreteScript = concreteObj.GetComponent<ConcreteObject>();

            // 데이터 전달 (속도, 유지시간 등)
            concreteObj.GetComponent<ConcreteObject>().Initialize(
                isHorizontal,
                _controller.centerPoint.position,
                _controller.Data
            );

            // 컨트롤러 명단에 등록
            _controller.RegisterConcrete(concreteScript);

            yield return new WaitForSeconds(_controller.Data.concreteInterval);

            yield return _controller.StartCoroutine(_controller.MoveBossTo(currentBoss, startPos, _controller.Data.bossMoveDuration));

            // 다음 패턴까지 대기
            yield return new WaitForSeconds(_controller.Data.patternWaitTime);
        }

        // 모든 생성이 끝난 후, 회수 옵션이 켜져있으면 일괄 회수
        if (_doRetract)
        {
            _controller.RetractAllConcretes();
        }
    }
}