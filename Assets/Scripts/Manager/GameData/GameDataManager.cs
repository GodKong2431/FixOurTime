using System.IO;
using UnityEngine;


public static class GameDataManager
{
    private static string savePath = Application.persistentDataPath + "/savedata.json";

    /// <summary>
    /// 지정한 경로에 json파일 생성 및 데이터 저장
    /// </summary>
    /// <param name="data">저장할 데이터 클래스</param>
    public static void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    //파일 로드
    public static GameData Load()
    {
        if (!File.Exists(savePath))
        {
            return new GameData();
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameData>(json);
    }
}
