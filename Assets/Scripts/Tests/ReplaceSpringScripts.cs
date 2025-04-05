using UnityEditor;
using UnityEngine;

public class ReplaceSpringScripts : EditorWindow
{
    [MenuItem("Tools/Wrap Springs Safely")]
    static void ReplaceSpringsWithParentContainers()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("No GameObject selected!");
            return;
        }

        GameObject root = Selection.activeGameObject;

        foreach (Spring spring in root.GetComponentsInChildren<Spring>(true))
        {
            Transform original = spring.transform;

            // Prevent double-wrapping if already wrapped
            if (original.parent != null && original.parent.name.StartsWith("Spring_"))
                continue;

            Transform oldParent = original.parent;

            // Create wrapper GameObject
            GameObject newWrapper = new GameObject("Spring_" + original.name);
            Undo.RegisterCreatedObjectUndo(newWrapper, "Create Spring Wrapper");

            newWrapper.transform.SetParent(oldParent, false);
            newWrapper.transform.localPosition = original.localPosition;
            newWrapper.transform.localRotation = original.localRotation;
            newWrapper.transform.localScale = original.localScale;

            Undo.SetTransformParent(original, newWrapper.transform, "Reparent Spring Part");

            original.localPosition = Vector3.zero;
            original.localRotation = Quaternion.identity;
            original.localScale = Vector3.one;

            // OPTIONAL: Remove physics components from the spring part
            RemoveComponent<Rigidbody>(original.gameObject);
            RemoveComponent<SpringJoint>(original.gameObject);
            RemoveComponent<Spring>(original.gameObject);

            Debug.Log($"Wrapped {original.name} under {newWrapper.name}");
        }

        Debug.Log("✅ Spring wrapping complete. Animator references should be intact.");
    }

    static void RemoveComponent<T>(GameObject obj) where T : Component
    {
        T comp = obj.GetComponent<T>();
        if (comp != null)
        {
            Undo.DestroyObjectImmediate(comp); // safer than DestroyImmediate in editor tools
        }
    }
}
