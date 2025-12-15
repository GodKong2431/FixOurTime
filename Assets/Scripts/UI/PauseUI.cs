using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private PauseButton _pause;
    [SerializeField] private Panel _panel;
    [SerializeField] private QuitCheckPanel _quitCheckPanel;
    [SerializeField] private MainCheckPanel _mainCheckPanel;


    private PausePresenter _pausePresenter;

    private void Start()
    {
        _pausePresenter = new PausePresenter(_pause, _panel, _quitCheckPanel, _mainCheckPanel);
    }
}
