using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
실행 순서(중요) 이거 이해 안하면 진짜 이해 불가능 안그러면 대머리 될듯


1. BuildTableNameToTypeMape() 실행
    - 프로젝트에 있는 모든 클
        
 */







public class CSVTableManager : MonoBehaviour
{
    private static Dictionary<string, Type> TableNameToTypeMap;

    private static void BuildTableNameToTypeMape()
    {
        TableNameToTypeMap = new Dictionary<string, Type>();

        //현재의 프로그램(AppDomain.CurrentDomain)에 저장되어 있던 어셈블리들(컴파일 된 스크립트, 오브젝트나 컴포넌트 등)의 주소를 저장함
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        //어셈블리들 중에 TableBase를 상속받은 클래스를 가져올려고 하니까 미리 타입을 캐싱해둠
        Type tableBasrType = typeof(CSVTableBase);

        //가져온 어셈블리들을 하나씩 열람해봄
        foreach (Assembly assembly in assemblies)
        {
            //클래스이면서 추상클래스가 아니고 TableBase를 상속받은 클래스를 가져와서 IEnumerable<Type>으로 저장
            IEnumerable<Type> rowTypes = assembly.GetTypes().Where(t => t.IsClass && t.IsAbstract == false && t.IsSubclassOf(tableBasrType));

            //타입 이름에 Data를 제거하고 딕셔너리 인덱서를 사용해서 키에 값을 저장 없다면 추가
            foreach (Type type in rowTypes)
            {
                string tableName = type.Name.Replace("Data", "");
                TableNameToTypeMap[tableName] = type;
            }
        }

        Debug.Log($"{TableNameToTypeMap.Count}개의 TRow매핑 완료");
    }
}
