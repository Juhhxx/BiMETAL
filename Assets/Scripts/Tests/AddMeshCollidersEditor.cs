using UnityEngine;
using UnityEditor;

public class AddMeshCollidersEditor : EditorWindow
{
    [MenuItem("Tools/Add Mesh Colliders to Selected")]
    static void AddMeshColliders()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("No GameObject selected!");
            return;
        }

        GameObject root = Selection.activeGameObject;
        int count = 0;

        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (meshFilter && meshRenderer && child.GetComponent<MeshCollider>() == null)
            {
                if ( child.GetComponent<Rigidbody>() == null )
                    Undo.AddComponent<Rigidbody>(child.gameObject);
                if ( child.GetComponent<SpringJoint>() == null )
                    Undo.AddComponent<SpringJoint>(child.gameObject);
                if ( child.GetComponent<Spring>() == null )
                    Undo.AddComponent<Spring>(child.gameObject);

                MeshCollider collider = Undo.AddComponent<MeshCollider>(child.gameObject);
                collider.convex = true;
                count++;
            }
        }

        Debug.Log($"Added {count} MeshColliders to '{root.name}' and its children.");
    }
}
