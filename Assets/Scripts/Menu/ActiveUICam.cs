using UnityEngine;

public class ActiveUICam : MonoBehaviour
{
    [field:SerializeField] public static Camera ActiveUICamera { get; private set; }

    private void OnEnable()
    {
        if ( ActiveUICamera == null )
            ActiveUICamera = GetComponent<Camera>();
    }

    private void OnDisable()
    {
        if (ActiveUICamera == GetComponent<Camera>())
            ActiveUICamera = null;
    }

    private void OnDestroy()
    {
        if ( ActiveUICamera != null )
            ActiveUICamera = null;
    }

    private void OnApplicationQuit()
    {
        if ( ActiveUICamera != null )
            ActiveUICamera = null;
    }
}