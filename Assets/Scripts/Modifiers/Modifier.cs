using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Modifier")]
public class Modifier : ScriptableObject
{
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public int Weight { get; private set; }
    [field: SerializeField] public bool NonWalkable { get; private set; }
    [field: SerializeField] public bool Dynamic { get; private set; }

    public void GenerateRandomValues()
    {
        Color = new Color(Random.value, Random.value, Random.value);
        Weight = Random.Range(1, 3);
    }
    public override string ToString() => $"C({Color}),, W({Weight})";
}
