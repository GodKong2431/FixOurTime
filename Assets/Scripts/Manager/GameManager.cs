using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    [Header("플레이어 관리")]
    Player _player;
    public Player Player
    {
        get
        {
            if (_player == null) _player = FindAnyObjectByType<Player>();
            return _player;
        }
    }

    [Header("멈춤관리")]
    bool _isPaused = false;
    public bool IsPaused => _isPaused;

    protected override void Awake()
    {
        base.Awake();
        //필요시 게임시작후 플레이어 참조
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if(_isPaused)
        {
            Time.timeScale = 0f;
            Debug.Log("일시정지");
        }
        else
        {
            Time.timeScale = 1f;

        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

}