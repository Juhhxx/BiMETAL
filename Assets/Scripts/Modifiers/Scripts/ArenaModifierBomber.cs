using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierBomber", menuName = "Modifier/Arena/Bomber")]
public class ArenaModifierBomber : ArenaModifierAbstract
{
    [SerializeField] private GameObject _missile;
    [SerializeField] private CharController _character;
    [SerializeField] private float _fireRate;

    private Transform _target;
    private float _timer;
    private Vector3 position;
    private Vector3 rotation;

    public override void ActivateModifier()
    {
        _target = FindAnyObjectByType<PlayerMovement>().transform;

        position = Vector3.zero;
        position.y = 10f;

        rotation = Vector3.zero;
        rotation.x = -45f;

        Debug.Log($"STARTING MODIFIER {name}");
    }

    public override void UpdateModifier()
    {
        if (_timer < _fireRate)
        {
            _timer += Time.deltaTime;
        }
        else if (_timer >= _fireRate)
        {
            Shoot();
            _timer = 0.0f;
        }
    }

    private void Shoot()
    {
        float rot = Random.Range(0f, 360f);
        rotation.y = rot;

        MissileController ctrl = Instantiate(_missile, position,
                                            Quaternion.Euler(rotation))
                                .GetComponent<MissileController>();

        ctrl.SetTarget(_target);
        ctrl.SetOwner(_character);
    }
}