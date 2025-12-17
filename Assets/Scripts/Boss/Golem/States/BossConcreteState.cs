using UnityEngine;
using System.Collections;

public class BossConcreteState : BossState
{
    private int _count;

    public BossConcreteState(BossController controller, int count) : base(controller)
    {
        _count = count;
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

            // 1. 보스 등장
            yield return _controller.StartCoroutine(_controller.MoveBossTo(currentBoss, targetPos, _controller.Data.bossMoveDuration));
            yield return new WaitForSeconds(_controller.Data.patternWaitTime);

            // 2. 콘크리트 생성 위치 계산 
            Vector3 spawnOffset = Vector3.zero;

            
            if (isHorizontal)
                spawnOffset = Vector3.left * _controller.Data.concreteSpawnOffsetH;
            else
                spawnOffset = Vector3.up * _controller.Data.concreteSpawnOffsetV;

            Vector3 finalSpawnPos = currentBoss.position + spawnOffset;

            GameObject prefab = isHorizontal ? _controller.concreteHPrefab : _controller.concreteVPrefab;
            GameObject concrete = Object.Instantiate(prefab, finalSpawnPos, Quaternion.identity);

            // 데이터 전달 (속도, 유지시간 등)
            concrete.GetComponent<ConcreteObject>().Initialize(
                isHorizontal,
                _controller.centerPoint.position,
                _controller.Data.concreteDuration,
                _controller.Data.concreteMoveTime // 이동 시간도 데이터에서 받음
            );

            yield return new WaitForSeconds(0.5f); // 쏘고 잠깐 대기

            // 3. 퇴장
            yield return _controller.StartCoroutine(_controller.MoveBossTo(currentBoss, startPos, _controller.Data.bossMoveDuration));
            yield return new WaitForSeconds(_controller.Data.patternWaitTime);
        }
    }
}