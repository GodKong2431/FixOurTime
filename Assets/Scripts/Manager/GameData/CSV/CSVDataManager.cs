using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 데이터 가져오는 방법
// 데이터 통으로 가져오고 싶으면
// 예) Get<ItemTableData>("ItemTable")
// Get<데이터 클래스>(csv 이름)

// 한 줄만 뽑고 싶으면
// 인덱서 만들어 놔서 id만 인덱스로 넣어주면 한 줄 나옴
// 거기에 .찍으면 데이터 종류 나옴

public class CSVDataManager : SingleTon<CSVDataManager>
{
    string _csvSOPath = "Assets/Resources/CSV/CSVSO";

    string[] _files;

    private Dictionary<string, SOBase> CSVMap = new();

    protected override void Awake()
    {
        base.Awake();
        _files = GetCSVFileNames(_csvSOPath);
        CSVSOMapping();

        var a = Get<ItemTableData>("ItemTable");
        Debug.Log(a[201011].itemdesc);
    }

    public TableSOBase<T> Get<T>(string csvTableName)
        where T : TableBase
    {
        if(CSVMap[csvTableName] is TableSOBase<T>)
        {
            TableSOBase<T> result = CSVMap[csvTableName] as TableSOBase<T>;
            if(result.RowDict == null)
                result.BuildIndex();

            return result;
        }

        return null;
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
