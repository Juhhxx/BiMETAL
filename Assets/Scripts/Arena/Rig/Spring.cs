using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [field:SerializeField] public Rigidbody RigidBody { get; private set; }
    [field:SerializeField] public SpringJoint Joint { get; private set; }
    [field:SerializeField] public Collider Collider { get; private set; }
    [HideInInspector] public float WobbleStrength;

    public void SetComponents()
    {
        RigidBody = GetComponent<Rigidbody>();
        Joint = GetComponent<SpringJoint>();
        Collider = GetComponentInChildren<Collider>();
    }

    private void FixedUpdate()
    {
        if ( RigidBody.linearVelocity.magnitude > 1f )
        {
            Vector3 newAccel = RigidBody.linearVelocity.magnitude * WobbleStrength * Random.insideUnitSphere;
            Debug.Log("Applying random acceleration of: " + newAccel);
            RigidBody.AddForce(newAccel, ForceMode.Acceleration);
        }

        // think of adding a flocking kind of AI behavior, but to exploit it into being chaotic
    }
}
