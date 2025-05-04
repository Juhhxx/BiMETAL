using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // :p
    public static HashSet<string> Instances { get; private set; }
    private void Awake()
    {
        Instances ??= new();
        
        if ( ! Instances.Contains(gameObject.name) )
        {
            Debug.Log("Adding new DDOL: " + gameObject.name);
            Instances.Add(gameObject.name);
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
