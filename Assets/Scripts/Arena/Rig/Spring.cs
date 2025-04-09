using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [field:SerializeField] public Rigidbody RigidBody { get; private set; }
    [field:SerializeField] public SpringJoint Joint { get; private set; }
    [field:SerializeField] public Collider Collider { get; private set; }
    [HideInInspector] public float WobbleStrength;

    private Vector3 _initPos;
    private Quaternion _initRot;

    private void Awake()
    {
        _initRot = transform.localRotation;
        _initPos = transform.localPosition;
    }

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

    public void ResetSpring(float length)
    {
        StartCoroutine(Clip(length));
    }

    private IEnumerator Clip(float length)
    {
        RigidBody.linearVelocity = Vector3.zero;
        RigidBody.angularVelocity = Vector3.zero;
        // Joint.connectedBody = null;
        // RigidBody.isKinematic = true;
        Collider.enabled = false;

        yield return null;

        float t = 0f;

        while (t < length)
        {
            t += Time.deltaTime;

            transform.SetLocalPositionAndRotation(
                Vector3.Lerp(transform.localPosition, _initPos, t / length), 
                Quaternion.Slerp(transform.localRotation, _initRot, t / length));
            
            yield return null;
        }

        transform.SetLocalPositionAndRotation(_initPos, _initRot);
        Debug.Log("Ended lerping " + (_initPos == transform.localPosition) + " " + (_initRot == transform.localRotation));
    }
}
