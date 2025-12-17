using UnityEngine;
using System.Collections;

public class BossScrapState : BossState
{
    private int _count;

    public BossScrapState(BossController controller, int count) : base(controller)
    {
        _count = count;
    }

    public override void Enter() { }
    public override void Exit() { }

    public override IEnumerator Execute()
    {
        Transform currentBoss = _controller.wallBossObject;

        for (int i = 0; i < _count; i++)
        {
            Vector3 startPos = currentBoss.position;
            startPos.y = _controller.player.position.y;
            currentBoss.position = startPos;

            
            Vector3 appearPos = startPos + Vector3.left * _controller.Data.bossAppearDistance;

            // 1. 등장
            yield return _controller.StartCoroutine(_controller.MoveBossTo(currentBoss, appearPos, _controller.Data.bossMoveDuration));

            yield return new WaitForSeconds(1.0f); // 조준 시간

            // 2. 발사
            GameObject scrap = Object.Instantiate(_controller.scrapPrefab, currentBoss.position, Quaternion.identity);
            scrap.GetComponent<ScrapObject>().Initialize(Vector3.left, _controller.Data);

            yield return new WaitForSeconds(0.5f); // 발사 후 딜레이

            // 3. 퇴장
            yield return _controller.StartCoroutine(_controller.MoveBossTo(currentBoss, startPos, _controller.Data.bossMoveDuration));

            yield return new WaitForSeconds(1.0f); // 다음 발사 대기
        }
    }
}