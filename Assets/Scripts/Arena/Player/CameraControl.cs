using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Movement Parameters")]
    [Space(5)]
    [SerializeField] private GameObject _cameraRig;
    [SerializeField] private float      _resetRotationSpeed;
    [SerializeField] private float      _maxLookUpAngle;
    [SerializeField] private float      _maxLookDownAngle;
    [SerializeField] private bool       _lockOnPlayer;
    [SerializeField] private bool       _switchCameraModes;

    [Space(10)]
    [Header("Deocclusion Parameters")]
    [Space(5)]
    [SerializeField] private bool       _doDeocclusion;
    [SerializeField] private Transform  _deocclusionPivot;
    [SerializeField] private LayerMask  _deocclusionLayerMask;
    [SerializeField] private float      _deocclusionThreshold;
    [SerializeField] private float      _deocclusionSpeed;

    private Transform   _cameraTransform;
    private Vector3     _rotation;
    private Vector3     _position;
    private float       _zoomPosition;
    private Vector3     _deocclusionVector;
    private Vector3     _deocclusionPoint;

    void Start()
    {
        _cameraTransform    = _cameraRig.GetComponentInChildren<Camera>().transform;
        _rotation           = transform.localEulerAngles;
        _zoomPosition       = _cameraTransform.localPosition.z;
        _deocclusionVector  = new Vector3(0f, 0f, _deocclusionThreshold);
    }
    void Update()
    {
        UpdateRotation();
        UpdateHeight();

        if (_doDeocclusion)
            PreventOcclusion();

    }

    private void UpdateRotation()
    {
        bool cameraButtonCheck = InputManager.Camera();

        if (_lockOnPlayer != _switchCameraModes) cameraButtonCheck = !cameraButtonCheck;

        if (cameraButtonCheck)
        {
            _rotation = _cameraRig.transform.localEulerAngles;
            _rotation.y += InputManager.MouseX();

            _cameraRig.transform.localEulerAngles = _rotation;
        }
        // else
        //     ResetRotation();
    }
    private void ResetRotation()
    {
        if (_rotation.y != 0f)
        {
            if (_rotation.y < 180f)
                _rotation.y = Mathf.Max(0f, _rotation.y - _resetRotationSpeed * Time.deltaTime);
            else
                _rotation.y = Mathf.Min(360f, _rotation.y + _resetRotationSpeed * Time.deltaTime);

            _cameraRig.transform.localEulerAngles = _rotation;
        }
    }
    private void UpdateHeight()
    {
        _rotation = _cameraRig.transform.localEulerAngles;
        _rotation.x -= InputManager.MouseY();

        if (_rotation.x < 180f)
            _rotation.x = Mathf.Min(_maxLookDownAngle, _rotation.x);
        else
            _rotation.x = Mathf.Max(_maxLookUpAngle, _rotation.x);

        _cameraRig.transform.localEulerAngles = _rotation;
    }
    private void PreventOcclusion()
    {
        _deocclusionPoint = _cameraTransform.position - _cameraTransform.TransformDirection(_deocclusionVector);

        if (Physics.Linecast(_deocclusionPivot.position, _deocclusionPoint, out RaycastHit hitInfo, _deocclusionLayerMask.value))
        {
            _cameraTransform.position = hitInfo.point + _cameraTransform.TransformDirection(_deocclusionVector);
        }
        else
            RevertDeocclusion();
    }
    private void RevertDeocclusion()
    {
        _position = _cameraTransform.localPosition;

        if (_position.z > _zoomPosition)
        {
            // _position.z = Mathf.Max(_position.z - _deocclusionSpeed * Time.deltaTime, _zoomPosition);
            // _deocclusionPoint = transform.TransformPoint(_position) - _cameraTransform.TransformDirection(_deocclusionVector);
            _position.z = _zoomPosition;

            if (!Physics.Linecast(_deocclusionPivot.position, _deocclusionPoint))
                _cameraTransform.localPosition = _position;
        }
    }
}
