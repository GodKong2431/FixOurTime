#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Reflection;
using System.IO;

public static class CSVAutoBuildPipeline
{
    private static string _csvSOPath = "Assets/Resources/CSV/CSVSO";

    // 컴파일이나 리로드 직후 자동으로 호출되는 콜백 메서드
    [DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        // "CSV_PENDING"으로 저장된 키가있는지 체크 없으면 리턴
        if (!EditorPrefs.HasKey("CSV_PENDING"))
            return;

        // EditorPrefs에 저장되어 있던 문자열 PendingTableBuild타입으로 읽어와 pending에 저장
        PendingTableBuild pending = JsonUtility.FromJson<PendingTableBuild>(EditorPrefs.GetString("CSV_PENDING"));

        // EditorPrefs에 저장되어 있던 빌드요청 키 삭제
        EditorPrefs.DeleteKey("CSV_PENDING");

        // 저장된 SO클래스 이름으로 타입을 찾아 저장
        Type soType = FindType(pending.soClassName);

        // soType의 SO 인스턴스를 메모리에 생성함 (아직 생성된게 아님)
        ScriptableObject so = ScriptableObject.CreateInstance(soType);

        // 저장되어 있던 경로에 저장된 클래스이름의 SO 에셋 생성하고 경로 저장
        string soAssetPath = Path.Combine(_csvSOPath, pending.soClassName + ".asset");

        // 저장된 경로에 메모리상의 인스턴스 so를 실제 에셋으로 저장
        AssetDatabase.CreateAsset(so, soAssetPath);

        // so에서 TableSOBase에 들어가있는 data 클래스를 뽑아옴
        Type rowType = GetRowTypeFromSO(so);

        // CSVGenericTableEditor클래스에서 "GenerateIntoSO"라는 메서드를 찾아서 rowType타입 제네릭 메서드로 생성
        MethodInfo m = typeof(CSVGenericTableEditor).GetMethod("GenerateIntoSO", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(rowType);

        // m에서 호출하는데 호출 대상이 없으니 null
        // 메서드에 pending.csvPath 와 so를 인자로 전달해서 실행
        m.Invoke(null, new object[]{pending.csvPath, so});


        // SO 변경 사항을 저장
        AssetDatabase.SaveAssets();

        Debug.Log("CSV → SO 자동 생성 완료");
    }


    /// <summary>
    /// 현재 프로젝트 내에 문자열에 해당하는 클래스 타입이 있는지 체크하고 타입 반환하는 메서드
    /// </summary>
    /// <param name="name"> 클래스 이름 </param>
    /// <returns></returns>
    private static Type FindType(string name)
    {
        // 프로젝트 내에 있는 어셈블리 중에 이름과 같은 타입을 찾아 타입을 return
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = asm.GetType(name);

            // t 찾았으니 
            if (t != null) 
                return t;
        }

        // 다 순회 했는데 없으면 return
        return null;
    }

    /// <summary>
    /// SO가 상속한 제네릭 속의 data타입을 상속
    /// </summary>
    /// <param name="so"></param>
    /// <returns></returns>
    private static Type GetRowTypeFromSO(ScriptableObject so)
    {
        Type t = so.GetType();

        //t가 null이면 종료
        while (t != null)
        {
            // t가 제네릭 타입이고 t의 타입이 TableSOBase<>인지 체크
            // 그냥 t랑 체크하면 안됨 GetGenericTypeDefinition()하지 않은 코드는 TableSOBase<ItemData>는 다르다고 체크해버림
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(TableSOBase<>))
                return t.GetGenericArguments()[0]; //제네릭 안에 있는 첫 data클래스를 리턴함

            // 자신의 부모로 올라가면서 탐색
            t = t.BaseType;
        }

        // 순회가 끝났는데도 못찾았으면 null 반환
        return null;
    }
}
#endif
