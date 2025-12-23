using UnityEngine;

public class Angel : MonoBehaviour
{
    Coroutine _staytimeCoroutine;
    Stage3AngelPlatform _platform = null;

    public void StayTimeController(Stage3AngelPlatform platform)
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
