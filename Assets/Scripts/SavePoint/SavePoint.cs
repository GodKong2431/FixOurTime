using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class SavePoint : MonoBehaviour
{
    [Header("고유 ID(곂치지 않게 쓰기)")]
    [SerializeField] string _savePointID;

    BoxCollider2D _collider;
    private bool _isSaved = false;

    
    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();

        GameData data = GameDataManager.Load();
        if (data.actSavePoints.Contains(_savePointID))
        {
            SetAsSaved();
        }
    }

    private void SetAsSaved()
    {
        _isSaved = true;
        if(_collider != null) _collider.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isSaved) return;

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(player != null)
            {
                SavePorcess(player);
            }
        }
    }

    private void SavePorcess(Player player)
    {
        player.CheckPoint(transform.position);

        GameData data = GameDataManager.Load();
        if (!data.actSavePoints.Contains(_savePointID))
        {
            data.actSavePoints.Add(_savePointID);
            GameDataManager.Save(data);
        }

        SetAsSaved();
        Debug.Log($"{_savePointID} 이지점 저장 완료");
    }
}
