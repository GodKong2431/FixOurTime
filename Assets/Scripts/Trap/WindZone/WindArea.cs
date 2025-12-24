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
                float targetWindForce = _windPower * _maxAcceleration;

                _windSpeed = Mathf.MoveTowards(_windSpeed,targetWindForce,(_maxAcceleration / _accelerationDuraition) * Time.fixedDeltaTime);

                Vector2 windForce = new Vector2(_windSpeed, 0f);

                _player.Rb.AddForce(windForce, ForceMode2D.Force);
            }
            else
            {
                _windSpeed = _windPower;
            }
        }
    }
}
