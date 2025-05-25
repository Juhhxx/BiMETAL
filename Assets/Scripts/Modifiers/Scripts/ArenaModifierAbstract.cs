using UnityEngine;

public abstract class ArenaModifierAbstract : ScriptableObject
{
    public abstract void ActivateModifier();
    public virtual void UpdateModifier()
    {
        
    }
}
