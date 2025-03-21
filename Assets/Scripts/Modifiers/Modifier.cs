using UnityEngine;

public class Modifier : MonoBehaviour
{
    public Color Color { get; private set; }


    private void Start()
    {
        Color = new Color(Random.value, Random.value, Random.value);
    }
}
