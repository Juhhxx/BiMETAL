using System;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class CharController : MonoBehaviour
{
    [SerializeField][Expandable] private Character _characterBase;
    private Character _character;
    public Character Character => _character;

    public UnityEvent OnDamageTaken;

    private void Awake()
    {
        _character = _characterBase.InstantiateCharacter();
        
        OnDamageTaken ??= new UnityEvent();
    }
    private void Update()
    {
        if (_character.HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void GiveDamage(CharController c, float amount)
    {
        c.TakeDamage(amount);
        Debug.LogWarning($"{_character.Name} gave {amount} damage.");
    }
    public void TakeDamage(float amount)
    {
        _character.HP -= amount;
        OnDamageTaken?.Invoke();
        Debug.LogWarning($"{_character.Name} took {amount} damage.");
    }

}
