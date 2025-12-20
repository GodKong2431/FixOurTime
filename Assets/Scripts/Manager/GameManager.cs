using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    private Player _player;
    private GameData _gameData;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //씬로드함수 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //파괴될때 구독해제
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
         //인게임 씬이 로드되었을 때 Player를 찾아서 참조
        //_player = FindObjectOfType<Player>(); (끝까지 다 찾아봄)

        //유니티 버전업하면서 최적화된 함수 (플레이어 찾으면 검색중단함)
        _player = FindFirstObjectByType<Player>();
        Debug.Log(_player == null ? "Player 없음" : "Player 찾음");

        _gameData = GameDataManager.Load();

        if (_player != null)
        {
             //저장된 데이터를 Player에게 로드            
            _player.LoadPlayerData(_gameData);
        }
    }


}