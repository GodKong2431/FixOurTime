using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePresenter
{
    PauseButton _pauseButton;
    Panel _panel;
    ResumeButton _resumeButton;
    QuitGameButton _quitButton;
    MainMenuButton _mainButton;
    MainManuYes _mainManuYes;
    QuitManuYes _quitMenuYes;
    No _no;
    QuitGameText _quitGameText;
    MainManuText _mainManuText;

    

    public PausePresenter(
        PauseButton button,
        Panel popUp,
        ResumeButton resumeButton,
        QuitGameButton quitButton,
        MainMenuButton mainButton,
        MainManuYes mainManuYes,
        QuitManuYes quitManuYes,
        No no,
        QuitGameText quitGameText,
        MainManuText mainManuText        
        )


    {
        _pauseButton = button;
        _panel = popUp;
        _resumeButton = resumeButton;
        _quitButton = quitButton;
        _mainButton = mainButton;
        _mainManuYes = mainManuYes;
        _quitMenuYes = quitManuYes;
        _no = no;
        _quitGameText = quitGameText;
        _mainManuText = mainManuText;


        //PauseButton.cs  에 있는 함수들 구독
        _pauseButton._onPauseClicked += Pause;
        _pauseButton._onResume += Resume;
        
        //Panel.cs 
        _panel._onResume += Resume;
        //_panel._onMainManu += MainMenu;
        //_panel._onQuitGame += QuitGame;

        _mainManuYes._mainManuYes += MainCheckYes;
        _quitMenuYes._quitManuYes += QuitCheckYes;
        _no._no += CheckNo;

    }

    public void Dispose()   //구독해제 함수
    {
        _pauseButton._onPauseClicked -= Pause;
        _pauseButton._onResume -= Resume;

        _panel._onResume -= Resume;
        //_panel._onMainManu -= MainMenu;
        //_panel._onQuitGame -= QuitGame;

        _mainManuYes._mainManuYes -= MainCheckYes;
        _no._no -= CheckNo;


    }

    // 우측상단 Pause 버튼
    void Pause()
    {
        Time.timeScale = 0f;    //게임 일시정지
        _pauseButton.gameObject.SetActive(false);    //퍼즈버튼 비활성화

        //Panel.cs 에 있는 팝업창 활성화 함수
        _panel.Show();
        _resumeButton.Show();
        _quitButton.Show();
        _mainButton.Show();
        Debug.Log($"TimeScale :" + Time.timeScale);
    }


    // Pause 클릭시 나오는 첫번째 패널 버튼들
    void Resume()
    {        
        Time.timeScale = 1f;
        _panel.Hide();
        _resumeButton.Hide();
        _quitButton.Hide();
        _mainButton.Hide();
        _mainManuYes.Hide();
        _quitMenuYes.Hide();
        _no.Hide();
        _quitGameText.Hide();
        _mainManuText.Hide();
        _pauseButton.gameObject.SetActive(true);
        Debug.Log($"TimeScale :" + Time.timeScale);
    }

    //void MainMenu()
    //{
    //    //첫번째 패널 버튼들 끄기
    //    _resumeButton.Hide();
    //    _quitButton.Hide();
    //    _mainButton.Hide();
    //
    //    //예 아니오 버튼 키기
    //    
    //    _mainManuYes.Show();
    //    
    //    
    //}
    //
    //void QuitGame()
    //{
    //    //첫번째 패널 버튼들 끄기
    //    _resumeButton.Hide();
    //    _quitButton.Hide();
    //    _mainButton.Hide();
    //
    //    //예 아니오 버튼 키기
    //           
    //
    //}

    

    //Main Manu 클릭시 나오는 확인 패널 버튼들

    void MainCheckYes()
    {
        Time.timeScale = 1f;    //일시정지 풀어주고 넘어가기
        SceneManager.LoadScene("LHS_Main"); //메인씬 이름
        Debug.Log($"TimeScale :" + Time.timeScale);
    }

    void QuitCheckYes()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }

    void CheckNo()
    {
        //아니오 누르면 원상복귀
        _resumeButton.Show();
        _quitButton.Show();
        _mainButton.Show();
    }
}
