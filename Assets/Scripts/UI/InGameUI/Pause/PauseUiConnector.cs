using UnityEngine;

public class PauseUiConnector : MonoBehaviour
{
    [SerializeField] private PauseButton _pause;
    [SerializeField] private Panel _panel;
    [SerializeField] private ResumeButton _resumeButton;
    [SerializeField] private QuitGameButton _quitGameButton;
    [SerializeField] private MainMenuButton _mainMenuButton;
    [SerializeField] private MainManuYes _mainManuYes;
    [SerializeField] private QuitManuYes _quitManuYes;
    [SerializeField] private No _no;
    [SerializeField] private QuitGameText _quitGameText;
    [SerializeField] private MainManuText _mainManuText;




    private PausePresenter _pausePresenter;

    private void Start()
    {
        _pausePresenter = new PausePresenter(
            _pause,
            _panel,
            _resumeButton,
            _quitGameButton,
            _mainMenuButton,
            _mainManuYes,
            _quitManuYes,
            _no,
            _quitGameText,
            _mainManuText            
            );
    }

    private void OnDestroy()
    {
        _pausePresenter?.Dispose(); //구독해제
    }
}
