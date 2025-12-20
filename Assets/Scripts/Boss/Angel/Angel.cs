using UnityEngine;

public class Angel : MonoBehaviour
{
    Coroutine _staytimeCoroutine;
    Stage3BossPlatform _platform = null;

    public void StayTimeController(Stage3BossPlatform platform)
    {
        if(_platform == platform) return;

        if (_staytimeCoroutine != null)
        {
            StopCoroutine(_staytimeCoroutine);
        }

        _platform = platform;
        _staytimeCoroutine = StartCoroutine(platform.CheckStayTime());
    }
}
