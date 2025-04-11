using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharController))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Colliders Parameters")]
    [Space(5)]
    [SerializeField] private CollisionDetector  _attackCollider;
    [SerializeField] private CollisionDetector  _dashAttackCollider;
    [SerializeField] private CollisionDetector  _jumpAttackCollider;

    [Space(10)]
    [Header("Camera Parameters")]
    [Space(5)]
    [SerializeField] private bool _lockCamera;

    [Space(10)]
    [Header("Combo Parameters")]
    [Space(5)]
    [SerializeField] [Range(0,1)] private float _maxComboTime = 1.0f;
    [SerializeField] private float              _onAirDistance;
    [SerializeField] private LayerMask          _groundLayer;
    private int     _attackCounter;
    private float   _comboTimer;
    private bool    _isOnAir;
    private bool    _isDashing;
    
    [Space(10)]
    [Header("Attack Events")]
    [Space(5)]
    public UnityEvent OnSlash;
    public UnityEvent OnSlashCombo;
    public UnityEvent OnDashSlashCombo;
    public UnityEvent OnJumpSlashCombo;

    // Game Object Components 
    private CameraControl   _cameraControl;
    private CharController  _character;

    private void Start()
    {
        _character = GetComponent<CharController>();
        _comboTimer = _maxComboTime;
    }
    private void OnEnable()
    {
        _attackCollider.OnCollisionEnter        += DoAttack;
        _dashAttackCollider.OnCollisionEnter    += DoAttack;
        _jumpAttackCollider.OnCollisionEnter    += DoAttack;
    }
    private void OnDisable()
    {
        _attackCollider.OnCollisionEnter        -= DoAttack;
        _dashAttackCollider.OnCollisionEnter    -= DoAttack;
        _jumpAttackCollider.OnCollisionEnter    -= DoAttack;
    }
    private void Update()
    {
        Attack();
        CheckOnAir();
    }

    private void Attack()
    {
        string attackName = "None";

        if (Input.GetButtonDown("Attack"))
        {
            if (_isOnAir)
            {
                OnJumpSlashCombo?.Invoke();
                attackName = "JumpCombo";
            }
            else if (_isDashing)
            {
                OnDashSlashCombo?.Invoke();
                attackName = "DashCombo";
            }
            else if (_attackCounter == 0)
            {
                _attackCounter++;
                OnSlash?.Invoke();
                attackName = "Slash1";
            }
            else if (_attackCounter >= 1 && _comboTimer > 0)
            {
                _attackCounter++;
                OnSlashCombo?.Invoke();
                attackName = "Slash2";

                ResetComboTimer();
            }

            Debug.Log($"Combo Timer : {_comboTimer} Attack : {attackName}");
        }

        CountComboTimer();
    }
    private void CountComboTimer()
    {
        if (_attackCounter >= 1 && _comboTimer > 0)
        {
            _comboTimer -= Time.deltaTime;
        }
        else if (_comboTimer <= 0)
        {
            ResetComboTimer();
        }
    }
    private void ResetComboTimer()
    {
        _comboTimer = _maxComboTime;
        _attackCounter = 0;
    }
    private void DoAttack(object sender, OnCollisionEventArgs e)
    {
        Collider self = e.self;
        Collider other = e.other;
        CharController otherChar = other.gameObject.GetComponent<CharController>();

        if (otherChar != null)
        {
            _character.GiveDamage(otherChar, CalculateAttackDamage(self, other));
        }

        Debug.Log($"Collision detected with {e.other.gameObject.name} from {gameObject.name}");
    }
    private float CalculateAttackDamage(Collider self, Collider other)
    {
        float finalDamage;

        if (_isOnAir)
        {
            Vector3 selfCorrected = self.transform.position;
            selfCorrected.y = 0f;

            Vector3 otherCorrected = other.transform.position;
            otherCorrected.y = 0f;

            float distance = Vector3.Distance(selfCorrected,otherCorrected);

            finalDamage = Mathf.Floor((-(distance * distance) * 5f) + 100);
            Debug.LogWarning($"finalDamage : {finalDamage}");
        }
        else if (_isDashing)
        {
            finalDamage = _character.Character.SpecialAttack;
        }
        else
        {
            finalDamage = _character.Character.BaseAttack ;
        }
        Debug.LogWarning($"finalDamage : {finalDamage}");
        return finalDamage;
    }
    private void CheckOnAir()
    {
        Debug.DrawLine(transform.position, transform.position + (-transform.up * _onAirDistance), Color.blue);;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, _onAirDistance))
        {
            _isOnAir = hit.collider.gameObject.layer == _groundLayer;
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }
        else
        {
            _isOnAir = true;
        }
    }
    public void SetDashing(bool set)
    {
        _isDashing = set;
    }

    
}
