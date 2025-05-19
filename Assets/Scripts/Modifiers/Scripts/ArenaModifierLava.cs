using UnityEngine;

[CreateAssetMenu(fileName = "ModifierLava", menuName = "Modifier/Arena/Lava")]
public class ArenaModifierLava : ArenaModifierAbstract
{
    public override void ActivateModifier()
    {
        Debug.Log($"STARTING MODIFIER {name}");
    }
}