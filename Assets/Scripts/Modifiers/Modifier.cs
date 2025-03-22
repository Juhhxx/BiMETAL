using UnityEngine;

public class Modifier : MonoBehaviour
{
    public Color Color { get; private set; }
    public int Weight { get; private set; }


    private void Start()
    {
        Color = new Color(Random.value, Random.value, Random.value);
        Weight = Random.Range(1, 3);
    }
}
