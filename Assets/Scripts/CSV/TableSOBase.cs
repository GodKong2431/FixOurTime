using System;
using System.Collections.Generic;
using UnityEngine;

// SO의 베이스가 되는 추상 클래스
// 제네릭으로 어떤 타입 테이블이든 리스트로 저장 가능
public abstract class TableSOBase<TRow> : SOBase
    where TRow : TableBase
{
    public List<TRow> rows = new();

    private Dictionary<int, TRow> _rowDictInt;
    private Dictionary<string, TRow> _rowDictStr;


    public IReadOnlyDictionary<int, TRow> RowDictInt => _rowDictInt;
    public IReadOnlyDictionary<string, TRow> RowDictStr => _rowDictStr;

    public void BuildIndex()
    {
        _rowDictInt = new();
        _rowDictStr = new();

        foreach (var row in rows)
        {
            _rowDictInt[row.id] = row;
        }
    }

    public TRow this[int Key]
    {
        get
        {
            return RowDictInt[Key];
        }
    }

    public TRow this[string Key]
    {
        get
        {
            return RowDictStr[Key];
        }
    }
}
