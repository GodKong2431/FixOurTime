using UnityEngine;

public class MainUIConnector : MonoBehaviour
{
    [SerializeField] private GameStart _newGameButton;
    [SerializeField] private LoadButton _loadSavePointButton;
    [SerializeField] private MainPanel _mainPanel;
    [SerializeField] private LoadCheckYes _loadCheckYes;
    [SerializeField] private LoadCheckNo _loadCheckNo;




    private MainUIPresenter _mainUIPresenter;

    private void Start()
    {
        _mainUIPresenter = new MainUIPresenter(
            _mainPanel,
            _newGameButton,
            _loadSavePointButton,
            _loadCheckYes,
            _loadCheckNo
            );
    }

    private void OnDestroy()
    {
        _mainUIPresenter?.Dispose(); //구독해제
    }
}
