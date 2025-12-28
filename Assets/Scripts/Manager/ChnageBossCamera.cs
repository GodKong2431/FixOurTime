using Unity.Cinemachine;
using UnityEngine;

public class ChnageBossCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _stageCam;
    [SerializeField] private CinemachineCamera _bossCam;


    public void ChangeBossChamera()
    {
        if(!_bossCam.gameObject.activeSelf)
            _bossCam.gameObject.SetActive(true);

        _stageCam.Priority = 0;
        _bossCam.Priority = 10;
    }

    public void ChageStageCamera()
    {
        if (_bossCam.gameObject.activeSelf)
            _bossCam.gameObject.SetActive(false);

        _stageCam.Priority = 10;
        _bossCam.Priority = 0;
    }
}
