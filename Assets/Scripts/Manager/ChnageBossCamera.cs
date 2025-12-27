using Unity.Cinemachine;
using UnityEngine;

public class ChnageBossCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _stageCam;
    [SerializeField] private CinemachineCamera _bossCam;


    public void ChangeChamera()
    {
        if(!_bossCam.gameObject.activeSelf)
            _bossCam.gameObject.SetActive(true);


        int temp = _stageCam.Priority;
        _stageCam.Priority = _bossCam.Priority;
        _bossCam.Priority = temp;

    }
}
