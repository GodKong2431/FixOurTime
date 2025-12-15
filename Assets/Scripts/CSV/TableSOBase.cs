using System.Collections.Generic;
using UnityEngine;
// SO의 베이스가 되는 추상 클래스
// 제네릭으로 어떤 타입 테이블이든 리스트로 저장 가능
public abstract class TableSOBase<TRow> : ScriptableObject
    where TRow : TableBase
{
    public List<TRow> rows = new();
}
