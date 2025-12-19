using UnityEngine;

public class WindArea : MonoBehaviour
{
    [Header("바람 강도")]
    [SerializeField] private float _windPower = 0.001f;
    [SerializeField] private float _accelerationDuraition = 1;
    [SerializeField] private float _maxAcceleration = 2;

    private float _windSpeed;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Player _player))
        {
            if(!_player.IsGrounded)
            {
                float maxWindSpeed = _windPower * _maxAcceleration;

                float accelPerFixedFrame = maxWindSpeed / _accelerationDuraition * Time.fixedDeltaTime;

                _windSpeed = Mathf.MoveTowards(_windSpeed, maxWindSpeed, accelPerFixedFrame);

                _player.Rb.linearVelocity = new Vector2(_windSpeed, _player.Rb.linearVelocity.y);

                Debug.Log(_windSpeed);
            }
            else
            {
                _windSpeed = _windPower;
            }
        }
    }
}
