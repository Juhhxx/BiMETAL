using UnityEngine;

public class Tabletop_camera : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private float _rotSpeed = 50f;
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _movSpeed = 10f;
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 15f;
    [SerializeField] private bool _rotate = true;
    [SerializeField] private float _easeTime = 0.05f;

    private void Awake()
    {
        if (_cam == null)
            _cam = Camera.main;

        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);
        _currentZoom = _cam.orthographicSize;
        _targetZoom = _currentZoom;

        _pitch = 40f;
        HandleRotation(true);

        _targetPosition = transform.position;
    }

    private void Update()
    {
        HandleZoom();
        if (_rotate)
            HandleRotation();
        // To restrict this movement create a collider that is always the same size a the tabletop
        // but sits somewhat above it, and then make sure the came stays within it, and change
        // the near based on that size too,
        // if this creates problems have an overlay camera that renders just the tabletop stuff
        HandleMovement();
    }

    private float _currentZoom;
    private float _targetZoom;
    private float _zoomVelocity;
    private void HandleZoom()
    {
        float change = InputManager.CamZoom() * _zoomSpeed;
        _targetZoom = Mathf.Clamp(_targetZoom - change, _minZoom, _maxZoom);

        _currentZoom = Mathf.SmoothDamp(_currentZoom, _targetZoom, ref _zoomVelocity, _easeTime * _zoomSpeed);
        _cam.orthographicSize = _currentZoom;

        // Probably temporary
        _cam.nearClipPlane = Mathf.Min(-50f, -_currentZoom);
    }

    private float _pitch;
    private float _pitchVelocity = 0f;
    private float _yawVelocity = 0f;
    private void HandleRotation(bool start = false)
    {
        if (InputManager.CamRotDown() || start)
        {
            float horizontalRotation = InputManager.CamRot().x * _rotSpeed;
            float verticalRotation = -InputManager.CamRot().y * _rotSpeed;

            float targetYaw = transform.eulerAngles.y + horizontalRotation * Time.deltaTime;
            float targetPitch = Mathf.Clamp(_pitch + verticalRotation * Time.deltaTime, 2f, 88f);

            _pitch = Mathf.SmoothDamp(_pitch, targetPitch, ref _pitchVelocity, _easeTime );
            float smoothedYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref _yawVelocity, _easeTime );

            transform.rotation = Quaternion.Euler(_pitch, smoothedYaw, 0);
        }
    }

    private Vector3 _targetPosition;
    private Vector3 _velocity = Vector3.zero;
    private void HandleMovement()
    {
        if (InputManager.CamMovDown())
        {
            Vector3 moveDirection = Vector3.zero;

            if (Mathf.Abs(InputManager.CamMov().x) > 1f)
            {
                float horizontalMove = InputManager.CamMov().x * _movSpeed * Time.deltaTime;
                moveDirection += transform.right * horizontalMove;
            }

            if (Mathf.Abs(InputManager.CamMov().y) > 1f)
            {
                float verticalMove = InputManager.CamMov().y * _movSpeed * Time.deltaTime;
                moveDirection += transform.up * verticalMove;
            }

            _targetPosition -= moveDirection;
        }
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _easeTime * _movSpeed);
    }
}