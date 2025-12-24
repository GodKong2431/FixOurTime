using UnityEngine;

public class MoveNextStage : MonoBehaviour
{
    [SerializeField] string _nextStageName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneChanger.Instance.ChangeScene(_nextStageName, false, false);
        }
    }
}
