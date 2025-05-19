using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Modifier", menuName = "Modifier/Tabletop")]
public class Modifier : ScriptableObject
{
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public int Weight { get; private set; }
    [field: SerializeField] public bool NonWalkable { get; private set; }
    [field: SerializeField] public bool Unavailable { get; private set; }
    [field: SerializeField] public bool Dynamic { get; private set; }

    [field: SerializeField] public ArenaModifierAbstract ArenaModifier { get; private set; }

    public Modifier Clone()
    {
        Modifier clone = Instantiate(this);
        clone.hideFlags = HideFlags.HideAndDontSave;
        return clone;
    }
    public override string ToString() => $"C({Color}),, W({Weight})";
    public void ActivateModifier() => ArenaModifier.ActivateModifier();
}
