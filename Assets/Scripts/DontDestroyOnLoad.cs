using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // :p
    [field:SerializeField] public static HashSet<string> Instances { get; private set; }
    public static HashSet<GameObject> InstancesGO { get; private set; }
    private void Awake()
    {
        Instances ??= new();
        InstancesGO ??= new();
        
        if ( ! Instances.Contains(gameObject.name) )
        {
            Debug.Log("Adding new DDOL: " + gameObject.name);
            Instances.Add(gameObject.name);
            InstancesGO.Add(gameObject);
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if ( InstancesGO.Contains(gameObject) )
        {
            Instances.Remove(gameObject.name);
            InstancesGO.Remove(gameObject);
        }
    }
}
