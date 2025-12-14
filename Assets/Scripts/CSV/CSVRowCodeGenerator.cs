#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;


public static class CSVRowCodeGenerator
{
    /// <summary>
    /// 위치에 row cs를 생성하는 메서드
    /// </summary>
    /// <param name="csvPath"> 읽어올 csv 파일의 경로 </param>
    /// <param name="outputPath"> 파일 생성할 경로 </param>
    /// <param name="className"> 클래스 이름 </param>
    public static void GenerateRow(string csvPath, string outputPath, string className)
    {
        // File.ReadAllLines
        // CSV파일을 줄단위로 나누어서 배열로 저장함
        string[] lines = File.ReadAllLines(csvPath);

        // 1번 인덱스에는 헤더 2번 인덱스는 자료형이니까
        // 헤더와 타입을 배열로 나눔
        string[] headers = lines[1].Split(',');
        string[] types = lines[2].Split(',');

        #region 자동으로 만들어질 코드
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("[Serializable]");
        sb.AppendLine($"public class {className} : TableBase");
        sb.AppendLine("{");
        
        // 타입과 헤더로 변수 생성
        for (int i = 1; i < headers.Length; i++)
        {
            sb.AppendLine($"    public {types[i]} {headers[i]};");
        }

        sb.AppendLine("}");
        #endregion

        // Path.GetDirectoryName(outputPath) : 문자열에 경로만 추출
        // 예) Assets/Scripts/Table/ItemTableSO.cs  ===>>>  Assets/Scripts/Table

        // Directory.CreateDirectory : 해당 경로로 폴더 생성
        // 이미 폴더가 존재하면 아무 작업도 하지않음
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));



        // File.WriteAllText( 파일 저장할 경로, 파일에 쓸 내용, 인코딩 방식 )
        // 지정한 경로에 파일을 생성하고 내용을 작성 (위에 작성했던 자동으로 만들어 질 코드를 경로에 만듬)
        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);



        // 에디터에 파일이 바뀌었다고 알림을 보내는 기능 cs파일을 인식하고 컴파일함
        AssetDatabase.Refresh();
    }
}
#endif
