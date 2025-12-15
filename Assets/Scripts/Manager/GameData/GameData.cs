using UnityEngine;

[System.Serializable]
public class GameData
{
    //플레이어 현재 체력 > 게임을 껏다 키거나 할때 쓰기위한용도
    public float currentHp;

    //플레이어 위치 좌표
    public Vector2 playerPos;

    // 사망시에 풀피로 로드하기 위한 최대 체력값
    public float maxHp;

    //기본 생성자, 데이터 파일이없을때 사용될 기본값 일단임시로 위치 제로로했는데 시작포인트 정하면 거기로수정바람
    public GameData()
    {
        currentHp = 100f;
        maxHp = 100f;
        playerPos = Vector2.zero;
    }

    //마지막 저장 씬 넣을거면 추가

}
