using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // :p
    public static DontDestroyOnLoad Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
