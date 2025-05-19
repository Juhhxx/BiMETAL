using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public string   Name;
    public float    HP;
    public float    BaseAttack;
    public float    SpecialAttack;
    public Faction  Faction;
    public GameObject CharacterPrefab;

    public Character InstantiateCharacter()
    {
        Character newC = CreateInstance<Character>();

        newC.Name               = Name;
        newC.HP                 = HP;
        newC.BaseAttack         = BaseAttack;
        newC.SpecialAttack      = SpecialAttack;
        newC.Faction            = Faction;
        newC.CharacterPrefab    = CharacterPrefab;

        return newC;
    }    
}
