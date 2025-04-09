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

            if ( meshFilter && meshRenderer )
            {


                // Transform original = spring.transform;
                Transform springParent = child.parent;

                if ( springParent != null )
                {
                    if ( !springParent.name.StartsWith("Spring_") )
                    {
                        // Create wrapper GameObject
                        GameObject newWrapper = new("Spring_" + child.name);
                        Undo.RegisterCreatedObjectUndo(newWrapper, "Create Spring Wrapper");

                        newWrapper.transform.SetParent(springParent, false);
                        newWrapper.transform.SetLocalPositionAndRotation(child.localPosition, child.localRotation);
                        newWrapper.transform.localScale = child.localScale;

                        Undo.SetTransformParent(child, newWrapper.transform, "Reparent Spring Part");

                        child.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                        child.localScale = Vector3.one;

                        Debug.Log($"Wrapped {child.name} under {newWrapper.name}");

                        springParent = newWrapper.transform;
                    }
                    if ( springParent.GetComponent<Rigidbody>() == null )
                        Undo.AddComponent<Rigidbody>(child.gameObject);
                    if ( springParent.GetComponent<SpringJoint>() == null )
                        Undo.AddComponent<SpringJoint>(child.gameObject);
                    if ( springParent.GetComponent<Spring>() == null )
                        Undo.AddComponent<Spring>(child.gameObject);

                    count++;
                }
                // Add box colliders manually
            }
        }

        Debug.Log($"Added or modified {count} springs to '{root.name}' and its children.");
    }
}
