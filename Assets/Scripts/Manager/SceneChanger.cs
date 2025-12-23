using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : SingleTon<SceneChanger>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void LoadGameFromSave()
    {
        GameData data = GameDataManager.Load();

        if (string.IsNullOrEmpty(data.sceneName))
        {
            Debug.Log("저장된 데이터없음 스테이지1시작");
            ChangeScene("Stage1", false);
        }
        else
        {
            StartCoroutine(LoadStageCoroutine(data.sceneName, true));
        }
    }

    public void ChangeScene(string sceneName,bool useSavePos = false)
    {
        StartCoroutine(LoadStageCoroutine(sceneName, useSavePos));
    }

    private IEnumerator LoadStageCoroutine(string sceneName,bool useSavePos)
    {
        //비동기 불러오기
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        //로딩 100퍼된건지 확인용
        while (!op.isDone)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame(); //로드후 한프레임 더 대기


        if (useSavePos)
        {
            GameData data = GameDataManager.Load();
            Player player = FindAnyObjectByType<Player>();
            if(player != null)
            {
                player.LoadPlayerData(data);
            }
            else
            {
                Debug.LogWarning("플레이어없음");
            }
        }

        if (CinemachinCamManager.Instance != null)
        {
            CinemachinCamManager.Instance.Reconnect();
        }
    }

    public void ChangeSceneFromButton(string sceneName)
    {
        // 버튼으로 씬을 바꿀 때는 보통 처음부터 시작하는 경우가 많으므로 false 전달
        // 혹은 상황에 따라 true를 넣고 싶다면 메서드를 하나 더 만듭니다.
        ChangeScene(sceneName, false);
    }
    public void ChangeSceneFromButtonWithSave(string sceneName)
    {
        ChangeScene(sceneName, true);
    }
}
