using System;
using System.Collections.Generic;
using UnityEngine;


public class SOBase : ScriptableObject
{

}


// SO의 베이스가 되는 추상 클래스
// 제네릭으로 어떤 타입 테이블이든 리스트로 저장 가능
public abstract class TableSOBase<TRow> : SOBase
    where TRow : TableBase
{
    public List<TRow> rows = new();

    private Dictionary<int, TRow> _rowDict;


    public void BuildIndex()
    {
        _rowDict = new();
        foreach (var row in rows)
        {
            Debug.Log(row);
            _rowDict[row.id] = row;
        }
    }

    public TRow GetIdRow(int id)
    {
        if (_rowDict != null && _rowDict.TryGetValue(id, out TRow row))
            return row;
        return null;
    }
}
