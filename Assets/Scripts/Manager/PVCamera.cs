using UnityEngine;

public class PVCamera : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;

    private void Update()
    {
        transform.position += Vector3.up * _speed * Time.deltaTime;
    }
}
