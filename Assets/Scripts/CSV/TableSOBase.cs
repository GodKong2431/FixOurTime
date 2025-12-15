using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class SOBase : ScriptableObject
{
}


// SO의 베이스가 되는 추상 클래스
// 제네릭으로 어떤 타입 테이블이든 리스트로 저장 가능
public abstract class TableSOBase<TRow> : SOBase
    where TRow : TableBase
{
    public List<TRow> rows = new();

    private Dictionary<int, TRow> _rowDict;

    public IReadOnlyDictionary<int, TRow> RowDict => _rowDict;

    public void BuildIndex()
    {
        _rowDict = new();
        foreach (var row in rows)
        {
            Debug.Log(row);
            _rowDict[row.id] = row;
        }
    }

    public TRow this[int Key]
    {
        get
        {
            return RowDict[Key];
        }
    }
}
