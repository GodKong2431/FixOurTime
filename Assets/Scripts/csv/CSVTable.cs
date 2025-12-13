using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CSVTable이라는 커스텀 자료구조 생성
/// 기본 적으로 딕셔너리의 형태이며 키와 밸류는 제네릭으로 생성됨
/// 값을 변경할 수 없고 생성했을 때의 값을 변경 불가능
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TRow"></typeparam>
public class CSVTable<TKey, TRow>
    where TRow : CSVTableBase
{
    //실제로 저장되는 딕셔너리 자료구조
    //여기서 readonly는 값이 불변이 되는 것이 아닌 참조 위치가 불변이 되는 것이다
    private readonly Dictionary<TKey, TRow> _data;

    //딕셔너리의 값을 외부에서 수정이 불가능하게 하고 읽기만 가능하기 위해 IReadOnlyDictionary 타입으로 
    public IReadOnlyDictionary<TKey, TRow> Data => _data;


    //외부에서 테이블 수정이 불가능하기 때문에 생성할 떄 값을 받아와 저장함
    public CSVTable( Dictionary<TKey, TRow> data)
    {
        _data = data;
    }


    //인덱서 클래스 인스턴스에 인덱스 붙이면 TRow 반환
    public TRow this[TKey id]
    {
        //인덱서로 CSVTable[지정한 키] 입력시 값을 찾아보고 있으면 반환
        get
        {
            if (_data.TryGetValue(id, out TRow row))
            {
                return row;
            }
            else
            {
                Debug.LogError($"존재하지 않는 ID : {id}");
                return null;
            }
        }
    }
}
