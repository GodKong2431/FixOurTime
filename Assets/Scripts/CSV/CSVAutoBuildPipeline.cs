#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Reflection;
using System.IO;

public static class CSVAutoBuildPipeline
{
    [DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        if (!EditorPrefs.HasKey("CSV_PENDING"))
            return;

        var pending = JsonUtility.FromJson<PendingTableBuild>(
            EditorPrefs.GetString("CSV_PENDING")
        );
        EditorPrefs.DeleteKey("CSV_PENDING");

        Type soType = FindType(pending.soClassName);
        ScriptableObject so = ScriptableObject.CreateInstance(soType);

        string soAssetPath = Path.Combine(
            pending.outputFolder,
            pending.soClassName + ".asset"
        );

        AssetDatabase.CreateAsset(so, soAssetPath);

        Type rowType = GetRowTypeFromSO(so);

        MethodInfo m = typeof(CSVGenericTableEditor)
            .GetMethod("GenerateIntoSO", BindingFlags.Public | BindingFlags.Static)
            .MakeGenericMethod(rowType);

        m.Invoke(null, new object[]
        {
        pending.csvPath,
        so
        });

        AssetDatabase.SaveAssets();

        Debug.Log("CSV → SO 자동 생성 완료");
    }


    private static Type FindType(string name)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = asm.GetType(name);
            if (t != null) return t;
        }
        return null;
    }

    private static Type GetRowTypeFromSO(ScriptableObject so)
    {
        Type t = so.GetType();
        while (t != null)
        {
            if (t.IsGenericType &&
                t.GetGenericTypeDefinition() == typeof(TableSOBase<>))
                return t.GetGenericArguments()[0];
            t = t.BaseType;
        }
        return null;
    }
}
#endif
