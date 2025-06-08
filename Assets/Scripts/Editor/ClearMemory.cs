using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ClearMemory
{
    static ClearMemory()
    {
        EditorApplication.delayCall += () => ClearMemoryNow();
    }
    
    [MenuItem("Tools/Clear Unused Assets")]
    static void ClearMemoryNow()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        UnityEditor.SceneView.RepaintAll();
        Debug.Log("Requested GC and asset cleanup.");
    }
}