#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class CSVTableAutoBuilderWindow : EditorWindow //이걸 상속 받아야 에디터 윈도우로 사용 가능
{
    //텍스트 에셋(csv)를 받을 변수
    private TextAsset _csv;

    //폴더를 받을 변수
    private DefaultAsset _outputFolder;

    // 상단 메뉴에 적은 경로의 항목추가
    [MenuItem("Tools/CSV/Auto Build Table")]
    public static void OpenCSVBuilder() // 위의 항목 클릭시 실행되는 함수
    {
        // "CSV Auto Builder" 이름의 에디터 창을 여는 기능
        // CSVTableAutoBuilderWindow 기능의 창이 열림
        GetWindow<CSVTableAutoBuilderWindow>("CSV Auto Builder");
    }

    // 에디터 윈도우에 UI를 그리는 기능 (에디터가 열릴 때 호출 됨)
    private void OnGUI()
    {
        // EditorGUILayout.ObjectField(); : 오브젝트 형식을 받는 필드를 그림
        // "CSV"             : 왼쪽에 표시될 이름
        // _csv              : 현재 값
        // typeof(TextAsset) : 들어올 타입 설정
        // false             : 씬에 배치되어 있는 오브젝트 허용
        _csv = (TextAsset)EditorGUILayout.ObjectField("CSV", _csv, typeof(TextAsset), false);

        //위와 양식 동일
        _outputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Output Folder", _outputFolder, typeof(DefaultAsset), false);

        // _csv와 _outputFolder에 값이 선택되어 있다면 밑에 UI들을 켜줌
        GUI.enabled = _csv && _outputFolder;
        // 높이가 40짜리 버튼을 만들고 버튼을 클릭하면 true
        if (GUILayout.Button("Build", GUILayout.Height(40)))
            Build(); //밑에 정의한 메서드 실행
    }

    private void Build()
    {
        // 에디터에서 선택한 파일의 경로를 추출
        string csvPath = AssetDatabase.GetAssetPath(_csv);
        string folder = AssetDatabase.GetAssetPath(_outputFolder);

        // csvPath경로의 파일의 이름을 추출(확장자 제거)
        // 예) Assets/Data/ItemTable.csv ===>>> ItemTable
        string tableName = Path.GetFileNameWithoutExtension(csvPath);

        // class 이름과 so 이름을 정하기
        string dataName = tableName + "Data";
        string soName = tableName + "TableSO";

        // csv를 읽고 정해진 위치에 헤더를 추출한 cs 생성
        CSVRowCodeGenerator.GenerateRow(csvPath, $"{folder}/{dataName}.cs", dataName);

        // class이름과 경로 및 so 이름으로 so class 생성
        CSVTableSOCodeGenerator.GenerateTableSO(dataName, $"{folder}/{soName}.cs", soName);

        // EditorPrefs은 프로젝트 리로드 및 다시 시작했을 때도 정보를 저장해 놓는 에디터 기능
        // JSON으로 저장한 이유 : 여러 데이터 저장할건데 여러 데이터 묶어서 저장할거라 문자열로 저장해야 하는데 JSON이 편함
        // 다른 데이터 추가시 확장성도 좋음
        // 결론적으로는 컴파일 이후에 작업해야할 목록을 저장해 놓는 것
        EditorPrefs.SetString(
            "CSV_PENDING", 
            JsonUtility.ToJson(new PendingTableBuild{csvPath = csvPath,soClassName = soName,outputFolder = folder})
            );

        // 이렇게도 저장이 가능함
        // EditorPrefs.SetString("CSV_PATH", csvPath);
        // EditorPrefs.SetString("SO_NAME", soName);
        // EditorPrefs.SetString("OUTPUT_FOLDER", folder);
    }
}
#endif
