using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class CSVTableParser
{
    //column(행)의 Header가 적혀있는 row(행)의 인덱스
    private const int HeaderRowIndex = 1;
    //데이터가 시작되는 row(행)의 인덱스
    private const int DataStartRowIndex = 3;

    /// <summary>
    /// 커스텀으로 만든 CSVTable을 채워 넣는 메서드
    /// </summary>
    /// <typeparam name="TKey">키 타입</typeparam>
    /// <typeparam name="TRow">밸류 타입</typeparam>
    /// <param name="csvFile">읽어올 파일</param>
    /// <returns></returns>
    public static CSVTable<TKey, TRow> Parse<TKey, TRow>(TextAsset csvFile, string idColumnName)
        where TRow : CSVTableBase, new()
    {
        //파일이 비어있는지 체크
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 null입니다.");
            return null;
        }

        //1. CSV 내용 읽기 및 줄/셀 분리한 행<셀 값>
        List<string[]> rows = new List<string[]>();
        try
        {
            //윈도우에서 CSV파일에서는 줄바꿈이 될 때 \r\n을 적용하여 줄바꿈이 된다
            //그렇기 때문에 줄 단위로 split 할려면 \r과 \n 둘다 적어 텍스트에서 제거할 필요가 있음
            string[] lines = csvFile.text.Split( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //각 라인을 반복
            foreach (string line in lines)
            {
                //라인의 값을 콤마단위로 나누어 배열에 저장
                string[] cells = line.Split(',');
                //각 셀에 Trim 메서드 사용해서 앞, 뒤 공백을 없애고 중간의 공백이 여러개 반복될 경우 하나로 만듬
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i] = cells[i].Trim();
                }

                //콤마로 나눈 라인 행
                rows.Add(cells);
            }
        }
        catch
        {
            Debug.LogError("CSV 파일 읽기 오류");
            return null;
        }



        //2. 각 열에 무슨 데이터가 들어가 있는지 체크하기위한 columnName(Header)를 배열로 저장

        //테이블의 전체 행의 수가 정해놓은 헤더의 인덱스보다 작다면 return
        if (rows.Count <= HeaderRowIndex)
        {
            Debug.LogError($"CSV에 컬럼 헤더 {HeaderRowIndex}가 없습니다");
            return null;
        }

        //헤더 행을 컬럼명 배열로 취득
        string[] columnNames = rows[HeaderRowIndex];

        //사용자가 열람할려고 하는 헤더 이름이 저장된 columnNames(헤더이름배열)에 있는지 확인
        int idIndex = Array.IndexOf(columnNames, idColumnName);

        //IndexOf는 실패할 시에 -1을 반환하기 때문에 0이하로 체크하면 실패했는지 알 수 있음
        if (idIndex < 0)
        {
            Debug.LogError($"CSV에 id 컬럼 {idColumnName}가 없습니다");
            return null;
        }

        //4. 데이터 파싱 및 TRow 인스턴스 생성

        //CSVTable에 저장될 값
        Dictionary<TKey, TRow> allRowData = new Dictionary<TKey, TRow>();

        //데이터가 시작되는 행부터 반복 시작
        for (int i = DataStartRowIndex; i < rows.Count; i++)
        {
            //처리 할려는 행
            string[] row = rows[i];


            // 처리할려는 행의 길이나 값에 이상이 있는지 체크
            if (row.Length < columnNames.Length)
            {
                Debug.Log("행의 길이가 columnNames(헤더이름배열)의 길이보다 짧습니다");
                continue;
            }
            if (Utill.IsAllStringNullOrEmpty(row))
            {
                Debug.Log("행에 빈칸이나 null 값이 들어있습니다");
                continue;
            }
                

            //ID 추출 및 TKey타입으로 변환

            //id컬럼 문자열 값
            string idString = row[idIndex];

            TKey idValue = default;
            try
            {
                idValue = (TKey)Convert.ChangeType(idString, typeof(TKey));
            }
            catch
            {
                Debug.LogError($"id {idString} 변환 실패");
            }

            if (allRowData.ContainsKey(idValue))
            {
                Debug.LogError($"{idValue} 중복");
                return null;
            }

            //id 처리 완료

            //새로운 TRow 인스턴스 생성
            TRow newRow = new TRow();

            //인스턴스에 값 채우기
            for (int j = 0; j < columnNames.Length; j++)
            {
                //컬럼 이름
                string columnName = columnNames[j];
                //읽어온 문자열 값
                string stringValue = row[j];

                //TRow에 멤버가 존재하는지 체크해서 진행
                //if (memberMap.TryGetValue(columnName, out MemberInfo member))
                //{
                //    Type targetType = (member is PropertyInfo prop) ? prop.PropertyType : ((FieldInfo)member).FieldType;
                //    try
                //    {
                //        //현재는 문자열 값으로 되어있으니까 테이블에서 정의한 타입으로 변환해서 매칭시켜주기
                //        object convertedValue = ConvertValue(stringValue, targetType);
                //        if (member is PropertyInfo propInfo)
                //        {
                //            propInfo.SetValue(newRow, convertedValue);
                //        }
                //        else if (member is FieldInfo fieldInfo)
                //        {
                //            fieldInfo.SetValue(newRow, convertedValue);
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        Debug.Log($"값 변환 오류: 칼럼 '{columnName}' 의 값 '{stringValue}' 을(를) {targetType} 타입으로 변환할 수 없습니다. 오류: {e.Message}");
                //    }
                //}
            }

            allRowData.Add(idValue, newRow);
        }

        return new CSVTable<TKey, TRow>(allRowData);
    }
}
