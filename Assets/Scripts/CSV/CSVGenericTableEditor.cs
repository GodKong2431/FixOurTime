#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CSVGenericTableEditor
{
    /// <summary>
    /// CSV 파일을 읽어 지정된 SO에 CSV 데이터를 채워 넣는 메서드
    /// </summary>
    /// <typeparam name="TRow"> csv 한 열 </typeparam>
    /// <param name="csvPath"> csv 파일 경로 </param>
    /// <param name="so"> 데이터 채워 넣을 SO </param>
    public static void GenerateIntoSO<TRow>(string csvPath, TableSOBase<TRow> so)
        where TRow : TableBase, new()
    {
        // csv 파일을 파일 경로에서 불러온다
        TextAsset csv = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);

        // csv 파일 없으면 리턴
        if (csv == null)
        {
            Debug.LogError("CSV 찾지 못했음");
            return;
        }

        // 기존에 있던 so의 데이터 리스트 초기화
        so.rows.Clear();

        // CSV 내용과 so안에 리스트 전달해서 파싱
        Parse(csv.text, so.rows);

        // so가 바뀌었다고 전달해서 적용 시킴
        EditorUtility.SetDirty(so);
    }

    /// <summary>
    /// CSV 문자열을 파싱해서 so에 값을 하나씩 저장시킴
    /// </summary>
    /// <typeparam name="TRow"> 리스트에 저장될 클래스 </typeparam>
    /// <param name="csvText"> csv 내용 </param>
    /// <param name="output"> csv 저장 될 리스트 </param>
    private static void Parse<TRow>(string csvText, List<TRow> output)
    where TRow : TableBase, new()
    {
        string[] lines = csvText.Split('\n');

        string[] headers = lines[1]
            .Split(',')
            .Select(h => h.Trim())
            .ToArray();

        // 필드맵 (대소문자 무시)
        var fieldMap = new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in typeof(TRow).GetFields())
            fieldMap[f.Name] = f;

        for (int i = 3; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            TRow row = new TRow();

            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                if (!fieldMap.TryGetValue(headers[j], out var field))
                    continue;

                string raw = values[j].Trim();

                if (!TryConvert(raw, field.FieldType, out var converted))
                {
                    Debug.LogError($"[Parse Fail] Row:{i} Field:{field.Name} Raw:'{raw}'");
                    continue;
                }

                field.SetValue(row, converted);
            }

            output.Add(row);
        }
    }

    /// <summary>
    /// CSV 문자열 값을 저장된 타입으로 변환 시도
    /// </summary>
    /// <param name="raw"> csv에 적혀있던 값 </param>
    /// <param name="targetType"> 변환할 데이터 타입 </param>
    /// <param name="result"> 변환 성공된 데이터 </param>
    /// <returns></returns>
    private static bool TryConvert(string raw, Type targetType, out object result)
    {
        raw = raw.Trim();
        result = null;

        Type actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            if (actualType == typeof(bool))
            {
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

            if (actualType.IsEnum)
            {
                if (Enum.TryParse(actualType, raw, true, out var enumValue))
                {
                    result = enumValue;
                    return true;
                }

                return false;
            }

            result = Convert.ChangeType(raw, actualType);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

#endif
