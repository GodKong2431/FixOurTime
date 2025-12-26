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
    public override void Exit()
    {       
        Stage1Boss boss = _baseBoss as Stage1Boss;
        if (boss != null && boss.WallFistObject != null)
            boss.WallFistObject.gameObject.SetActive(false);
    }
    public override IEnumerator Execute()
    {
        Stage1Boss _boss = _baseBoss as Stage1Boss;
        if (_boss == null) yield break;

        Transform fist = _boss.WallFistObject;
        fist.gameObject.SetActive(true);

        for (int i = 0; i < _count; i++)
        {
            // 1. 위치 잡기 (플레이어 Y축 추적)
            Vector3 startPos = fist.position; // 보통 벽 안쪽 숨겨진 위치
            // X축은 고정(벽 안), Y축만 플레이어 따라감
            startPos.y = _boss.PlayerTarget.position.y;
            fist.position = startPos;

            Vector3 punchPos = startPos + Vector3.left * _boss.BossData.BossAppearDistance;

            // 2. 조준 
            yield return new WaitForSeconds(_boss.BossData.ScrapAimDelay);

            // 3. 펀치
            float punchSpeed = _boss.BossData.BossMoveDuration * 0.5f;
            yield return _boss.StartCoroutine(_boss.MoveBossTo(fist, punchPos, punchSpeed));

            // 4. 발사
            float fireOffset = 2.0f;
            Vector3 firePos = fist.position + (Vector3.left * fireOffset);

            GameObject scrap = Object.Instantiate(_boss.ScrapPrefab, firePos, Quaternion.identity);
            scrap.GetComponent<ScrapObject>().Initialize(Vector3.left, _boss.BossData);

            yield return new WaitForSeconds(_boss.BossData.ScrapFireDelay);

            // 5. 회수
            yield return _boss.StartCoroutine(_boss.MoveBossTo(fist, startPos, _boss.BossData.BossMoveDuration));

            yield return new WaitForSeconds(_boss.BossData.ScrapCycleWaitTime);
        }

        fist.gameObject.SetActive(false);
    }
}