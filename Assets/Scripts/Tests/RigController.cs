using System.Collections;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Sets up physics based rig, for more chaotic movement change spring's rigidBody masses to very different values.
/// </summary>
public class RigController : MonoBehaviour
{
    [SerializeField] private Rigidbody _mainRigidBody;
    [SerializeField] private Animator _animator;

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
    [SerializeField] private float _LimbLerpTime;


    [SerializeField] private float _AnimationSpring = 8000f;
    [SerializeField] private float _AnimationDamper = 12000f;
    [SerializeField] private float _AnimationMinDistance, _AnimationMaxDistance;
    [SerializeField] private float _AnimationTolerance = 0.0001f;
    [SerializeField] private float _AnimationLinearDamping = 4f, _AnimationAngularDamping = 4f;

    private void Start()
    {
        #if UNITY_EDITOR
        ApplySpringSettings();
        #endif

        _animator.enabled = false;

        ToggleRig();
    }

    #if UNITY_EDITOR
    public void ApplySpringSettings()
    {
        if (_joints == null || _joints.Length == 0)
        {
            Debug.LogWarning("No joints assigned!");
            return;
        }

        foreach (Spring joint in _joints)
        {
            if (joint == null)
                continue;

            joint.SetComponents();

            if (joint.RigidBody == null || joint.Joint == null)
            {
                Debug.LogWarning($"Missing Rigidbody or SpringJoint on {joint.name}");
                continue;
            }

            joint.WobbleStrength = _wobbleStrength;

            joint.Joint.spring = _springForce;
            joint.Joint.damper = _springDamper;
            joint.Joint.minDistance = _minAnchorDistance;
            joint.Joint.maxDistance = _maxAnchorDistance;
            joint.Joint.tolerance = _forceTolerance;
            joint.Joint.connectedBody = _mainRigidBody;
            // Physics.IgnoreCollision(GetComponentInChildren<Collider>(), joint.Collider);
            joint.Joint.connectedMassScale = 0f;


            joint.RigidBody.isKinematic = false;

            joint.Joint.autoConfigureConnectedAnchor = false;
            // connected anchor is father anchor, where the spring connects on this Rigidbody, in local space
            joint.Joint.anchor = joint.transform.InverseTransformPoint(joint.RigidBody.worldCenterOfMass);
            // anchor is self anchor, where the spring connects to the main body, in the main body’s local space
            joint.Joint.connectedAnchor = _mainRigidBody.transform.InverseTransformPoint(joint.transform.position);

            joint.Joint.enableCollision = true;
            joint.Joint.axis = Vector3.zero;

            joint.RigidBody.linearDamping = _linearDamping;
            joint.RigidBody.angularDamping = _angularDamping;

            // joint.RigidBody.mass = joint.GetComponentInChildren<MeshFilter>().mesh.vertexCount / 100;
            // Debug.Log("vertices now mass as: " + joint.RigidBody.mass);

            joint.RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            joint.RigidBody.useGravity = true;
        }
    }
    #endif

    private IEnumerator TurnOnPhysicsRig()
    {
        foreach( Spring joint in _joints )
        {
            // joint.Joint.connectedBody = _mainRigidBody;
            // joint.RigidBody.isKinematic = false;
            joint.Collider.enabled = true;

            joint.Joint.spring = _springForce;
            joint.Joint.damper = _springDamper;
            joint.Joint.minDistance = _minAnchorDistance;
            joint.Joint.maxDistance = _maxAnchorDistance;
            joint.Joint.tolerance = _forceTolerance;

            joint.RigidBody.linearDamping = _linearDamping;
            joint.RigidBody.angularDamping = _angularDamping;

            joint.enabled = true;
        }

        yield return new WaitForSeconds(_LimbLerpTime);

        _animator.enabled = false;
    }
    private IEnumerator TurnOnAnimationRig()
    {
        _animator.enabled = true;

        yield return new WaitForSeconds(_LimbLerpTime);

        foreach( Spring joint in _joints )
        {
            // joint.ResetSpring(_LimbLerpTime);
            joint.Collider.enabled = false;

            joint.Joint.spring = _AnimationSpring;
            joint.Joint.damper = _AnimationDamper;
            joint.Joint.minDistance = _AnimationMinDistance;
            joint.Joint.maxDistance = _AnimationMaxDistance;
            joint.Joint.tolerance = _AnimationTolerance;

            joint.RigidBody.linearDamping = _AnimationLinearDamping;
            joint.RigidBody.angularDamping = _AnimationAngularDamping;

            joint.enabled = false;
        }
    }

    private bool _physics = true;

    private void ToggleRig()
    {
        _physics = ! _physics;
        
        if ( _physics )
        {
            StartCoroutine(TurnOnAnimationRig());
        }
        else
        {
            StartCoroutine(TurnOnPhysicsRig());
        }

        Debug.Log("Toggle rig to: " + _physics);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ToggleRig();
    }
}
