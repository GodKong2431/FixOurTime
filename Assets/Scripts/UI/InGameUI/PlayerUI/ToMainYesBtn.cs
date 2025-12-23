using System;
using UnityEngine;
using UnityEngine.UI;

public class ToMainYesBtn : MonoBehaviour
{
    [SerializeField] string targetScene = "Title";
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            SceneChanger.Instance.ChangeScene(targetScene, false,false);
        });
    }
}
