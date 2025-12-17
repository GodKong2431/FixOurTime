using UnityEngine;

public class PlayerUiConnector : MonoBehaviour
{
    [SerializeField] private PlayerHpUI _playerHpUI;
    [SerializeField] private Player _player;

    private PlayerHpPresenter _plaerHpPresenter;

    private void Start()
    {    
        _plaerHpPresenter = new PlayerHpPresenter(_player, _playerHpUI);

        
    }

    private void OnDestroy()
    {
        _plaerHpPresenter.Dispose();
    }
}
