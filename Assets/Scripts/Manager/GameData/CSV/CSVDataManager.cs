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
    private readonly Dictionary<string, SOBase> _csvMap = new();

    protected override void Awake()
    {
        base.Awake();
        LoadAllCSVSO();
    }

    /// <summary>
    /// Resources/CSV/CSVSO 안에 있는 모든 CSV ScriptableObject 로드
    /// </summary>
    private void LoadAllCSVSO()
    {
        _csvMap.Clear();

        SOBase[] tables = Resources.LoadAll<SOBase>("CSV/CSVSO");

        if (tables == null || tables.Length == 0)
        {
            Debug.LogError("[CSVDataManager] CSV SO를 하나도 못 불러왔습니다.");
            return;
        }

        foreach (var table in tables)
        {
            if (_csvMap.ContainsKey(table.name))
            {
                Debug.LogWarning($"[CSVDataManager] 중복 CSV 이름 : {table.name}");
                continue;
            }

            _csvMap.Add(table.name, table);
        }

        Debug.Log($"[CSVDataManager] CSV Loaded Count : {_csvMap.Count}");
    }

    /// <summary>
    /// CSV 테이블 가져오기
    /// </summary>
    public TableSOBase<T> Get<T>(string csvTableName)
        where T : TableBase
    {
        if (!_csvMap.TryGetValue(csvTableName, out var soBase))
        {
            Debug.LogError($"[CSVDataManager] CSV 테이블 없음 : {csvTableName}");
            return null;
        }

        if (soBase is not TableSOBase<T> table)
        {
            Debug.LogError($"[CSVDataManager] CSV 타입 불일치 : {csvTableName}");
            return null;
        }

        // 인덱스 빌드 안 돼 있으면 자동 생성
        if (table.RowDictInt == null)
            table.BuildIndex();

        return table;
    }
}
