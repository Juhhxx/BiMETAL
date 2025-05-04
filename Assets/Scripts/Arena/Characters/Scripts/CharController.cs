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
    public UnityEvent OnDeath;

    private void Awake()
    {
        _character = _characterBase.InstantiateCharacter();
    }

    public void GiveDamage(CharController c, float amount)
    {
        if (c.Character.Faction != _character.Faction)
        {
            c.TakeDamage(amount);
            Debug.LogWarning($"{_character.Name} gave {amount} damage.");
        }
        else
            Debug.Log("No friendly fire!");
    }
    public void TakeDamage(float amount)
    {
        _character.HP -= amount;
        OnDamageTaken?.Invoke();
        Debug.LogWarning($"{_character.Name} took {amount} damage.");
        if (_character.HP <= 0)
        {
            OnDeath?.Invoke();
        }
    }

}
