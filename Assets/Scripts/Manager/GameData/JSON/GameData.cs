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

    //씬 정보
    public string sceneName;

    //카메라 위치 저장
    public Vector3 cameraPos; 

}
