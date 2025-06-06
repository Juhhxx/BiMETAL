using System;
using NaughtyAttributes;
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
    [ShowIf("_lockCamera")] [SerializeField] private float _lockTime;

    [Space(10)]
    [Header("Combo Parameters")]
    [Space(5)]
    [SerializeField] [Range(0,1)] private float _backstabbingThreshold;
    [SerializeField] [Range(0,1)] private float _maxComboTime = 1.0f;
    [SerializeField] private float              _onAirDistance;
    [SerializeField] private LayerMask          _groundLayer;
    private int     _attackCounter;
    private float   _comboTimer;
    private bool    _isOnAir;
    private bool    _isDashing;
    private Vector3 _corretcetPosition;
    private string  _attackName;

    
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
    private SphereCollider  _collider;

    private void Start()
    {
        _cameraControl  = GetComponent<CameraControl>();
        _character      = GetComponent<CharController>();
        _collider       = _jumpAttackCollider.gameObject.GetComponentInChildren<SphereCollider>();
        _comboTimer     = _maxComboTime;
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
        _attackName = "None";

        if (InputManager.Attack())
        {
            if (_isOnAir)
            {
                Debug.Log("Jump Attack");
                OnJumpSlashCombo?.Invoke();
                _attackName = "JumpCombo";
            }
            else if (_isDashing)
            {
                OnDashSlashCombo?.Invoke();
                _attackName = "DashCombo";
            }
            else if (_attackCounter == 0)
            {
                _attackCounter++;
                OnSlash?.Invoke();
                _attackName = "Slash1";
            }
            else if (_attackCounter >= 1 && _comboTimer > 0)
            {
                _attackCounter++;
                OnSlashCombo?.Invoke();
                _attackName = "Slash2";
            }

            Debug.Log($"Combo Timer : {_comboTimer} Attack : {_attackName}");
            Debug.LogWarning($"Attack Counter : {_attackCounter}");
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
            if (_lockCamera) _cameraControl.LockOnPoint(other.transform, _lockTime);
        }

        if (_attackCounter == 2) ResetComboTimer();
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

            float maxDistance = _collider.radius * _collider.transform.localScale.x;

            // finalDamage = Mathf.Floor((-(distance * distance) * 5f) + 100);
            finalDamage = _character.Character.SpecialAttack * (distance/maxDistance);
        }
        else if (_isDashing)
        {
            finalDamage = _character.Character.SpecialAttack;
        }
        else
        {
            finalDamage = _character.Character.BaseAttack * (1 + (0.25f * (_attackCounter - 1))) + 
            CalculateBackstabbingBonus(other.transform);
        }

        Debug.Log($"finalDamage : {finalDamage}");
        return finalDamage;
    }
    private float CalculateBackstabbingBonus(Transform enemy)
    {
        Vector3 playerFwr   = _attackCollider.transform.forward;
        Vector3 enemyFwr    = enemy.forward;

        float dot = Vector3.Dot(playerFwr, enemyFwr);
        dot = Mathf.Round(dot*10)/10;

        Debug.Log($"Confirming : {dot} > {_backstabbingThreshold} ? {dot > _backstabbingThreshold}");
        if (dot > _backstabbingThreshold)
        {
            Debug.Log($"BACKSTAB BONUS x{dot}");
            return (_character.Character.BaseAttack/2) * dot;
        }
        else
            return 0.0f;
    }
    private void CheckOnAir()
    {
        _corretcetPosition = transform.position;
        _corretcetPosition.y -= 1.0f;

        Debug.DrawLine(_corretcetPosition, _corretcetPosition + (-transform.up * _onAirDistance), Color.blue);;

        RaycastHit hit;

        if (Physics.Raycast(_corretcetPosition, -transform.up, out hit, _onAirDistance))
        {
            _isOnAir = hit.collider.gameObject.layer == _groundLayer;
            Debug.DrawLine(_corretcetPosition, hit.point, Color.red);
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
