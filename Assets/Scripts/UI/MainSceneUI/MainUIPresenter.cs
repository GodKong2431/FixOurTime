using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIPresenter : MonoBehaviour
{
    MainPanel _mainPanel;
    GameStart _newGameButton;
    LoadButton _loadSavePointButton;
    LoadCheckYes _loadCheckYes;
    LoadCheckNo _loadCheckNo;




    public MainUIPresenter(
        MainPanel panel,
        GameStart button,
        LoadButton popUp,
        LoadCheckYes yes,
        LoadCheckNo no
        )


    {
        _mainPanel = panel;
        _newGameButton = button;
        _loadSavePointButton = popUp;
        _loadCheckYes = yes;
        _loadCheckNo = no;
        


        //GameStart.cs  에 있는 함수들 구독
        _newGameButton._onGameStart += GameStart;

        //Load.cs
        _loadSavePointButton._onLoad += Load;

        //LoadCheckNo.cs
        _loadCheckNo._onLoadNo += CheckNo;

        //LoadCheckYes.cs
        _loadCheckYes._onLoadYes += CheckYes;
    }

    public void Dispose()   //구독해제 함수
    {
        _newGameButton._onGameStart -= GameStart;


    }

    // GameStart 버튼
    void GameStart()
    {
        //세이브 데이터가 존재한다면
        if (SaveDataExists())
        {
            //메인버튼들 비활성화
            _newGameButton.Hide();
            _loadSavePointButton.Hide();

            //확인창 패널 표시
            _mainPanel.Show();
        }
        else
        {
            //데이터가 없다면 인게임씬으로 넘어가기
            SceneManager.LoadScene("LHS_InGame");
        }
    }


    //Load 버튼
    void Load()
    {        
        //세이브 데이터가 존재한다면
        if (SaveDataExists())
        {
            //데이터 로드
            GameData data = GameDataManager.Load();
            //저장된씬으로 넘어가기
            SceneManager.LoadScene(data.sceneName);
            
        }
        else
        {
            //데이터가 없다면 원상복귀
            _mainPanel.Hide();
            _newGameButton.Show();
            _loadSavePointButton.Show();
        }
    }


    //새게임시작시 세이브데이터가있을시 확인창
    void CheckYes()
    {
        //새로운 데이터값 생성
        GameData data = new GameData();
        data.sceneName = "LHS_InGame";

        //기존 Json파일 덮어쓰기
        GameDataManager.Save(data);

        //1스테이지 씬 불러오기
        SceneManager.LoadScene("LHS_InGame");
    }

    void CheckNo()
    {
        //아니오 누르면 원상복귀
        _mainPanel.Hide();
        _newGameButton.Show();
        _loadSavePointButton.Show();
        
    }


    //데이터 존재확인 함수
    bool SaveDataExists()
    {
        return System.IO.File.Exists(Application.persistentDataPath + "/savedata.json");
    }
}
