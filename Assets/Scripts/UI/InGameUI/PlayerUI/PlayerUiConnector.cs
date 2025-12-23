using UnityEngine;

public class PlayerUiConnector : MonoBehaviour
{
    [SerializeField] private PlayerHpUI _playerHpUI;

    private PlayerHpPresenter _plaerHpPresenter;

    private void Start()
    {    
        _plaerHpPresenter = new PlayerHpPresenter(GameManager.Instance.Player, _playerHpUI);

        
    }

    private void OnDestroy()
    {
        _plaerHpPresenter.Dispose();
    }
}
