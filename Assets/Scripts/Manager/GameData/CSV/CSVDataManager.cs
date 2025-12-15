using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


// 데이터 가져오기 사용 방법
// csv 통으로 가져오고 싶을 떄
// CSVMap[가져올 데이터 csv이름] as TableSOBase<데이터 클래스> 
// 이렇게 하면 csv 통으로 가져오기임

// csv에서 id로 원하는 행 가져오고 싶을 때
// 가져온 csv 통 정보에
// 가져온 통 데이터.BuildIndex(); 로 자동 딕셔너리 id 맵핑 후
// 가져온 통 데이터.GetIdRow(원하는 id(int값));
// 이렇게 하면 id에 해당하는 행 가져와서 id에 해당하는 데이터 가져올 수 있음


public class CSVDataManager : MonoBehaviour
{
    string _csvSOPath = "Assets/Resources/CSV/CSVSO";

    string[] _files;

    public Dictionary<string, SOBase> CSVMap = new();

    private void Awake()
    {
        _files = GetCSVFileNames(_csvSOPath);
        CSVSOMapping();
    }

    private void Start()
    {
        Debug.Log((CSVMap["ItemTable"] as TableSOBase<ItemTableData>).GetIdRow(201011));
    }

    public string[] GetCSVFileNames(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return null;

        string[] files = Directory.GetFiles(folderPath, "*.asset");
        string[] result = new string[files.Length];


        for (int i = 0; i < files.Length; i++)
        {
            result[i] = Path.GetFileNameWithoutExtension(files[i]);
        }

        return result;
    }

    public void CSVSOMapping()
    {
        if(_files == null)
            return;
        foreach (string file in _files)
        {
            SOBase tableSO = Resources.Load<SOBase>($"CSV/CSVSO/{file}");

            if (tableSO != null)
            {
                CSVMap[file] = tableSO;
            }
        }
    }
}
