#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;

public static class CSVTableSOCodeGenerator
{
    public static void GenerateTableSO(string rowClassName, string outputPath, string soClassName)
    {
        //자동으로 만들 SO 스크립트 코드 작성
        #region 자동으로 만들어질 코드
        var sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine($"[CreateAssetMenu(menuName = \"TableSO/{rowClassName}\")]");
        sb.AppendLine($"public class {soClassName} : TableSOBase<{rowClassName}>");
        sb.AppendLine("{");
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
