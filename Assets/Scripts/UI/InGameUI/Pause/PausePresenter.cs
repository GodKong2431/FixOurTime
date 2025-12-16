using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePresenter
{
    PauseButton _pauseButton;
    Panel _panel;
    QuitCheckPanel _quitCheckPanel;
    MainCheckPanel _mainCheckPanel;

    public PausePresenter(PauseButton button, Panel popUp, QuitCheckPanel quitPopUp, MainCheckPanel mainPopUp)
    {
        _pauseButton = button;
        _panel = popUp;
        _quitCheckPanel = quitPopUp;
        _mainCheckPanel = mainPopUp;

        //PauseButton.cs        
        _pauseButton._onPauseClicked += Pause;

        //Panel.cs 에 있는 함수들 구독
        _panel._onResume += Resume;
        _panel._onMainManu += MainMenu;
        _panel._onQuitGame += QuitGame;

        //CheckPanel.cs 에 있는 함수들 구독
        _quitCheckPanel._quitCheckYes += QuitCheckYes;
        _quitCheckPanel._quitCheckNo += QuitCheckNo;

        //MainCheckPanel 에 있는 함수들 구독
        _mainCheckPanel._mainCheckYes += MainCheckYes;
        _mainCheckPanel._mainCheckNo += MainCheckNo;
    }

    // 우측상단 Pause 버튼
    void Pause()
    {
        Time.timeScale = 0f;    //게임 일시정지
        _pauseButton.gameObject.SetActive(false);    //퍼즈버튼 비활성화
        _panel.Show();   //Panel.cs 에 있는 팝업창 활성화 함수
        Debug.Log($"TimeScale :" + Time.timeScale);
    }


    // Pause 클릭시 나오는 첫번째 패널 버튼들
    void Resume()
    {
        Time.timeScale = 1f;
        _panel.Hide();
        _pauseButton.gameObject.SetActive(true);
        Debug.Log($"TimeScale :" + Time.timeScale);
    }

    void MainMenu()
    {
        _panel.Hide(); //패널끄기
        _mainCheckPanel.Show(); //확인창 띄우기
    }

    void QuitGame()
    {
        _panel.Hide(); //패널끄기
        _quitCheckPanel.Show(); //확인창 띄우기
    }

    //Quit Game 클릭시 나오는 확인 패널 버튼들
     
    void QuitCheckYes()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }

    void QuitCheckNo()
    {
        //아니오 누르면 원상복귀
        _quitCheckPanel.Hide();
        _panel.Show();
    }

    //Main Manu 클릭시 나오는 확인 패널 버튼들

    void MainCheckYes()
    {
        Time.timeScale = 1f;    //일시정지 풀어주고 넘어가기
        SceneManager.LoadScene("LHS_Main"); //메인씬 이름
        Debug.Log($"TimeScale :" + Time.timeScale);
    }

    void MainCheckNo()
    {
        _mainCheckPanel.Hide();
        _panel.Show();
    }
}
