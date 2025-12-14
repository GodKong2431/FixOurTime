#if UNITY_EDITOR
using System;
using System.Collections.Generic;
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
        // csv를 \n 단위로 스플릿해서 줄로 나눔
        string[] lines = csvText.Split('\n');

        // csv의 첫번째 줄은 헤더니까 1번 줄 콤마로 분리해서 저장
        string[] headers = lines[1].Split(',');

        // 필드맵을
        // 키로 필드의 이름
        // 밸류로 필드로 채움
        Dictionary<string, FieldInfo> fieldMap = new Dictionary<string, FieldInfo>();
        foreach (var f in typeof(TRow).GetFields())
            fieldMap[f.Name] = f;

        // 실제 데이터인 3행부터 데이터 파싱 시작
        for (int i = 3; i < lines.Length; i++)
        {
            // 한 라인이 비어 있다면 다음 줄 체크
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            // 라인을 , 단위로 각 칸의 데이터 배열에 저장
            string[] values = lines[i].Split(',');

            // 필드를 저장해 놓을 임시 클래스 생성
            TRow row = new TRow();

            // 헤더의 갯수만큼
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                // fieldMap에 매핑된 필드 이름 중에 headers[j]가 없으면 다음 필드 헤더 체크
                if (!fieldMap.TryGetValue(headers[j], out var field))
                    continue;

                // 문자열로 된 values[j]를 헤더에 적혀있던 FieldType으로 변환에 실패하면 다음꺼 체크
                if (!TryConvert(values[j], field.FieldType, out var converted))
                {
                    Debug.LogError($"[파싱 실패] 클래스:{i} 필드명:{field.Name} 값:{values[j]}");
                    continue;
                }

                // 변환된 값을 임시 클래스에 저장
                field.SetValue(row, converted);
            }

            // 작업이 끝난 줄을 리스트에 저장 시킴
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
        //값의 앞뒤 공백 없애고 중간에 공백이 연속이면 하나로 변경
        raw = raw.Trim();

        // 변환된 데이터 저장할 변수를 null로 초기화 해놓음
        result = null;

        try
        {
            // 타입이 문자열이면
            if (targetType == typeof(string))
            {
                // 바로 저장후 return
                result = raw;
                return true;
            }

            // 타입이 int면
            if (targetType == typeof(int))
            {
                // int로 변환
                result = int.Parse(raw);
                return true;
            }

            // 타입이 bool이면
            if (targetType == typeof(bool))
            {
                // StringComparison.OrdinalIgnoreCase 대소문자 구분하지 않겠다는 의미
                // 1이거나 "true"면 true 반환
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

                // 둘 다 아니면 실패
                return false;
            }

            // 타입이 이넘이면
            if (targetType.IsEnum)
            {
                // 타겟 타입의 enum으로 변경 true는 대소문자 무시하겠다는 의미
                result = Enum.Parse(targetType, raw, true);
                return true;
            }

            // 마지막으로 시도해 볼 코드 오브젝트 위에서 선언되지 않은 코드 변환 시도
            result = Convert.ChangeType(raw, targetType);
            return true;
        }
        catch
        {
            // 시도하다가 실패했으면 false
            return false;
        }
    }
}

#endif
