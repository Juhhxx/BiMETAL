using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Sets up physics based rig, for more chaotic movement change spring's rigidBody masses to very different values.
/// </summary>
public class SpringController : MonoBehaviour
{
    [SerializeField] private Rigidbody _mainRigidBody;

    [Label("All spring joints")]
    [SerializeField] private Spring[] _joints;

    [BoxGroup("Rigidbody Settings"), Label("The drag, or air resistance, removes weird air bounce")]
    [SerializeField] private float _linearDamping = 2f;

    [BoxGroup("Rigidbody Settings"), Label("The angular drag, if things spin too wildly")]
    [SerializeField] private float _angularDamping = 3;

    [BoxGroup("Spring Settings"), Label("How strongly it pulls toward the rest position, makes the spring pull quicker")]
    [SerializeField] private float _springForce = 220f;

    [BoxGroup("Spring Settings"), Label("How much it resists oscillations")]
    [SerializeField] private float _springDamper = 90f;

    [BoxGroup("Spring Settings"), Label("Minimum/Maximum anchor distance")]
    [SerializeField] private float _minAnchorDistance = 0.01f, _maxAnchorDistance = 0.05f;

    [BoxGroup("Spring Settings"), Label("Allowed slack before spring kicks in")]
    [SerializeField] private float _forceTolerance = 0.025f;

    [SerializeField] private float _wobbleStrength;

    #if UNITY_EDITOR
    private void Start()
    {
        ApplySpringSettings();
    }
    public void ApplySpringSettings()
    {
        if (_joints == null) return;

        foreach (Spring joint in _joints)
        {
            if (joint == null) continue;

            joint.WobbleStrength = _wobbleStrength;

            joint.Joint.spring = _springForce;
            joint.Joint.damper = _springDamper;
            joint.Joint.minDistance = _minAnchorDistance;
            joint.Joint.maxDistance = _maxAnchorDistance;
            joint.Joint.tolerance = _forceTolerance;

            joint.Joint.autoConfigureConnectedAnchor = false;
            // connected anchor is father anchor
            joint.Joint.connectedAnchor = joint.transform.localPosition;

            joint.Joint.enableCollision = true;
            joint.Joint.axis = Vector3.zero;

            joint.RigidBody.linearDamping = _linearDamping;
            joint.RigidBody.angularDamping = _angularDamping;

            joint.RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            joint.RigidBody.useGravity = true;

            // anchor is self anchor
            joint.Joint.anchor =  joint.RigidBody.centerOfMass;
        }

        TurnOnPhysicsRig();
    }
    #endif

    private void TurnOnPhysicsRig()
    {
        foreach( Spring joint in _joints )
        {
            joint.Joint.connectedBody = _mainRigidBody;
            joint.RigidBody.isKinematic = false;
        }

        // turn off animator after half a second or something
    }
}
