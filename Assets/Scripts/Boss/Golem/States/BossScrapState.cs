using UnityEngine;
using System.Collections;

public class BossScrapState : BossState
{
    private int _count;

    public BossScrapState(Stage1Boss _boss, int count) : base(_boss)
    {
        _count = count;
    }

    public override void Enter() { }
    public override void Exit() { }

    public override IEnumerator Execute()
    {
        Stage1Boss _boss = _baseBoss as Stage1Boss;
        if (_boss == null) yield break;

        Transform currentBoss = _boss.WallBossObject;

        for (int i = 0; i < _count; i++)
        {
            Vector3 startPos = currentBoss.position;
            startPos.y = _boss.PlayerTarget.position.y;
            currentBoss.position = startPos;

            
            Vector3 appearPos = startPos + Vector3.left * _boss.BossData.BossAppearDistance;

            // 1. 등장
            yield return _boss.StartCoroutine(_boss.MoveBossTo(currentBoss, appearPos, _boss.BossData.BossMoveDuration));

            yield return new WaitForSeconds(_boss.BossData.ScrapAimDelay); // 조준 시간

            // 2. 발사
            GameObject scrap = Object.Instantiate(_boss.ScrapPrefab, currentBoss.position, Quaternion.identity);
            scrap.GetComponent<ScrapObject>().Initialize(Vector3.left, _boss.BossData);

            yield return new WaitForSeconds(_boss.BossData.ScrapFireDelay); // 발사 후 딜레이

            // 3. 퇴장
            yield return _boss.StartCoroutine(_boss.MoveBossTo(currentBoss, startPos, _boss.BossData.BossMoveDuration));

            yield return new WaitForSeconds(_boss.BossData.ScrapCycleWaitTime); // 다음 발사 대기
        }
    }
}