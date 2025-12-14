#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CSVGenericTableEditor
{
    public static void Generate<TRow, TSO>(string csvPath, string soPath)
        where TRow : TableBase, new()
        where TSO : TableSOBase<TRow>
    {
        TextAsset csv = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);
        TSO so = AssetDatabase.LoadAssetAtPath<TSO>(soPath);

        if (csv == null || so == null)
        {
            Debug.LogError("CSV or SO not found");
            return;
        }

        so.rows.Clear();
        Parse(csv.text, so.rows);

        EditorUtility.SetDirty(so);
        AssetDatabase.SaveAssets();
    }

    public static void GenerateIntoSO<TRow>(
    string csvPath,
    TableSOBase<TRow> so
)
    where TRow : TableBase, new()
    {
        TextAsset csv = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);
        if (csv == null)
        {
            Debug.LogError("CSV not found");
            return;
        }

        so.rows.Clear();
        Parse(csv.text, so.rows);

        EditorUtility.SetDirty(so);
    }

    private static void Parse<TRow>(string csvText, List<TRow> output)
        where TRow : TableBase, new()
    {
        string[] lines = csvText.Split('\n');
        string[] headers = lines[1].Split(',');

        var fieldMap = new Dictionary<string, FieldInfo>();
        foreach (var f in typeof(TRow).GetFields())
            fieldMap[f.Name] = f;

        for (int r = 3; r < lines.Length; r++)
        {
            if (string.IsNullOrWhiteSpace(lines[r]))
                continue;

            string[] values = lines[r].Split(',');
            var row = new TRow();

            for (int c = 0; c < headers.Length && c < values.Length; c++)
            {
                if (!fieldMap.TryGetValue(headers[c], out var field))
                    continue;

                if (!TryConvert(values[c], field.FieldType, out var converted))
                {
                    Debug.LogError(
                        $"[CSV Parse Fail] Row:{r} Field:{field.Name} Value:{values[c]}"
                    );
                    continue;
                }

                field.SetValue(row, converted);
            }

            output.Add(row);
        }
    }
    private static bool TryConvert(string raw, Type targetType, out object result)
    {
        raw = raw.Trim();
        result = null;

        try
        {
            if (targetType == typeof(string))
            {
                result = raw;
                return true;
            }

            if (targetType == typeof(int))
            {
                result = int.Parse(raw);
                return true;
            }

            if (targetType == typeof(bool))
            {
                // CSV 관용 처리
                if (raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    return true;
                }
                if (raw == "0" || raw.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    result = false;
                    return true;
                }

                return false;
            }

            if (targetType.IsEnum)
            {
                result = Enum.Parse(targetType, raw, true);
                return true;
            }

            // fallback (float, double 등)
            result = Convert.ChangeType(raw, targetType);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

#endif
