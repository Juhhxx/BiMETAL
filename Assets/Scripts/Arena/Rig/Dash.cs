using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _force;
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.D))
            _rigidBody.AddForce(Vector3.forward * _force, ForceMode.Impulse);
        if ( Input.GetKey(KeyCode.S))
            _rigidBody.AddForce(Vector3.forward * _force, ForceMode.Force);
        if ( Input.GetKey(KeyCode.A))
            _rigidBody.AddForce(Vector3.forward * _force, ForceMode.VelocityChange);
    }
}
