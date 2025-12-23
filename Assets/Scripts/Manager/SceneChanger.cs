using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : SingleTon<SceneChanger>
{
    public bool _videoPlay = false; //영상 초기셋팅 꺼두기
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
            ChangeScene("stage1", false);
            _videoPlay = true;
        }
        else
        {
            StartCoroutine(LoadStageCoroutine(data.sceneName, true));
            _videoPlay = false;
        }
    }

    public void ChangeScene(string sceneName,bool useSavePos = false)
    {
        StartCoroutine(LoadStageCoroutine(sceneName, useSavePos));
    }

    private IEnumerator LoadStageCoroutine(string sceneName,bool useSavePos)
    {
        _videoPlay = true;
        //비동기 불러오기
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        //로딩 100퍼된건지 확인용
        while (!op.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f); //로드1초대기

        if (CinemachinCamManager.Instance != null)
        {
            CinemachinCamManager.Instance.Reconnect();
        }

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

        
    }

    //버튼용
    public void ChangeSceneFromButton(string sceneName)
    {
        ChangeScene(sceneName, false);
    }
    public void ChangeSceneFromButtonWithSave(string sceneName)
    {
        ChangeScene(sceneName, true);
    }
}
