using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private GameObject _cameraRig;
    [SerializeField] private float      _resetRotationSpeed;
    [SerializeField] private float      _maxLookUpAngle;
    [SerializeField] private float      _maxLookDownAngle;
    [SerializeField] private float      _zoomMinDistance;
    [SerializeField] private float      _zoomMaxDistance;
    [SerializeField] private float      _zoomDeceleration;
    [SerializeField] private bool       _doDeocclusion;
    [SerializeField] private Transform  _deocclusionPivot;
    [SerializeField] private LayerMask  _deocclusionLayerMask;
    [SerializeField] private float      _deocclusionThreshold;
    [SerializeField] private float      _deocclusionSpeed;

    private Transform   _cameraTransform;
    private Vector3     _rotation;
    private Vector3     _position;
    private float       _zoomAcceleration;
    private float       _zoomVelocity;
    private float       _zoomPosition;
    private Vector3     _deocclusionVector;
    private Vector3     _deocclusionPoint;

    void Start()
    {
        _cameraTransform    = _cameraRig.GetComponentInChildren<Camera>().transform;
        _rotation           = transform.localEulerAngles;
        _zoomVelocity       = 0f;
        _zoomPosition       = _cameraTransform.localPosition.z;
        _deocclusionVector  = new Vector3(0f, 0f, _deocclusionThreshold);
    }

    void Update()
    {
        UpdateRotation();
        UpdateHeight();
        UpdateZoom();

        if (_doDeocclusion)
            PreventOcclusion();
    }

    private void UpdateRotation()
    {
        if (Input.GetButton("Camera"))
        {
            _rotation = _cameraRig.transform.localEulerAngles;
            _rotation.y += Input.GetAxis("Mouse X");

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
        _rotation.x -= Input.GetAxis("Mouse Y");

        if (_rotation.x < 180f)
            _rotation.x = Mathf.Min(_maxLookDownAngle, _rotation.x);
        else
            _rotation.x = Mathf.Max(_maxLookUpAngle, _rotation.x);

        _cameraRig.transform.localEulerAngles = _rotation;
    }

    private void UpdateZoom()
    {
        UpdateZoomVelocity();
        UpdateZoomPosition();
    }

    private void UpdateZoomVelocity()
    {
        _zoomAcceleration = Input.GetAxis("Zoom");

        if (_zoomAcceleration != 0f)
            _zoomVelocity += _zoomAcceleration * Time.deltaTime;
        else if (_zoomVelocity > 0f)
        {
            _zoomVelocity -= _zoomDeceleration * Time.deltaTime;
            _zoomVelocity = Mathf.Max(0f, _zoomVelocity);
        }
        else
        {
            _zoomVelocity += _zoomDeceleration * Time.deltaTime;
            _zoomVelocity = Mathf.Min(0f, _zoomVelocity);
        }
    }

    private void UpdateZoomPosition()
    {
        if (_zoomVelocity != 0f)
        {
            _position = _cameraTransform.localPosition;
            _position.z += _zoomVelocity * Time.deltaTime;

            if (_position.z > -_zoomMinDistance || _position.z < -_zoomMaxDistance)
            {
                _position.z = Mathf.Clamp(_position.z, -_zoomMaxDistance, -_zoomMinDistance);
                _zoomVelocity = 0f;
            }

            _cameraTransform.localPosition = _position;
            _zoomPosition = _position.z;
        }
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
