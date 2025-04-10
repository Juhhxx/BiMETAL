using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Gameobjects")]
    [Space(5)]
    [SerializeField] private GameObject _playerModel;
    [SerializeField] private GameObject _cameraRig;

    [Space(10)]
    [Header("Player Movement Parameters")]
    [Space(5)]
    [SerializeField] private float _maxForwardSpeed;
    [SerializeField] private float _maxStrafeSpeed;
    [SerializeField] private float _maxBackwardSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTimer;
    [SerializeField] private LayerMask _ignoreWhenDashing;
    [SerializeField] private float _gravity;
    [SerializeField] private float _maxFallSpeed;

    [Space(10)]
    [Header("Player Events")]
    [Space(5)]
    public UnityEvent OnPlayerDashStart;
    public UnityEvent OnPlayerDashEnd;
    public UnityEvent OnPlayerJump;

    // References and Variables
    private CharacterController _controller;
    private Animator            _anim;
    private Vector3             _velocityHor;
    private Vector3             _velocityVer;
    private Vector3             _motion;
    private bool                _jump;
    private bool                _dash;
    private float               _dashCountDown;

    private void Start()
    {
        _controller     = GetComponent<CharacterController>();
        _anim           = GetComponent<Animator>();
        _velocityHor    = Vector3.zero;
        _velocityVer    = Vector3.zero;
        _motion         = Vector3.zero;
        _jump           = false;
        _dash           = false;

        HideCursor();
    }
    private void Update()
    {
        UpdateRotation();
        CheckForJump();
        CheckForDash();

        _anim.SetBool("IsGrounded",_controller.isGrounded);        
        _anim.SetBool("IsDashing",_dash);        
    }
    private void FixedUpdate()
    {
        UpdateVelocityHor();
        UpdateVelocitYVer();
        UpdateDash();
        UpdatePosition();
    }

    private void UpdateRotation()
    {
        float rotation = 0;

        rotation = InputManager.MouseX();
    
        _playerModel.transform.Rotate( 0f, rotation, 0f);
    }
    private void CheckForJump()
    {
        if (InputManager.Jump() && _controller.isGrounded)
        {
            _jump = true;
            OnPlayerJump?.Invoke();
        }
    }
    private void UpdateVelocityHor()
    {
        float forwardAxis = InputManager.Forward();
        float strafeAxis = InputManager.Strafe();

        _velocityHor.x = strafeAxis  * _maxStrafeSpeed;

        if (forwardAxis > 0f)
        {
            _velocityHor.z = forwardAxis * _maxForwardSpeed;

            if (_velocityHor.magnitude > _maxForwardSpeed)
                _velocityHor = _velocityHor.normalized * _maxForwardSpeed;
        }
        else if (forwardAxis < 0f)
        {
            _velocityHor.z = forwardAxis * _maxBackwardSpeed;

            if (_velocityHor.magnitude > _maxBackwardSpeed)
                _velocityHor = _velocityHor.normalized * _maxBackwardSpeed;
        }
        else
            _velocityHor.z = 0f;
    }
    private void UpdateVelocitYVer()
    {
        if (_jump)
        {
            _velocityVer.y = _jumpSpeed;
            _jump = false;
        }
        else if (_controller.isGrounded)
            _velocityVer.y = -0.1f; // CHANGE ISGROUNDED TO RAYCAST
        else if (_velocityVer.y > -_maxFallSpeed)
        {
            _velocityVer.y += (-_gravity) * Time.fixedDeltaTime;

            _velocityVer.y = Mathf.Max(_velocityVer.y, -_maxFallSpeed);
        }
    }
    private void UpdatePosition()
    {
        _motion = (_velocityHor + _velocityVer) * Time.fixedDeltaTime;

        _motion = _playerModel.transform.TransformVector(_motion);

        _controller.Move(_motion);

        // Debug.Log($"Speed: {_motion}");
    }
    // Dash Control
    private void UpdateDash()
    {
        if (_dash)
        {
            _velocityHor.z  = _dashSpeed;
            CountDashTimer();
        }
    }
    private void CheckForDash()
    {
        if (InputManager.Dash())
        {
            _dash = true;
            OnPlayerDashStart?.Invoke();
            _controller.excludeLayers = _ignoreWhenDashing;
            _dashCountDown  = _dashTimer;
        }
    }
    private void CountDashTimer()
    {
        if (_dash && _dashCountDown > 0)
        {
            _dashCountDown -= Time.fixedDeltaTime;
        }
        else if (_dashCountDown <= 0)
        {
            ResetDashTimer();
        }
    }
    private void ResetDashTimer()
    {
        _dashCountDown = _dashTimer;
        _dash = false;
        _controller.excludeLayers = 0;
        OnPlayerDashEnd?.Invoke();
    }
    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
