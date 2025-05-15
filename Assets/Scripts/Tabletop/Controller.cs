using UnityEngine;

public class Controller : MonoBehaviour
{
    private void OnDisable()
    {
        Debug.LogWarning("Disabled: " + gameObject.name + "'s " + this);
    }
}
