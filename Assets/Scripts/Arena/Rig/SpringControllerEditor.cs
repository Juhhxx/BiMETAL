using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RigController))]
public class SpringControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RigController controller = (RigController)target;

        EditorGUILayout.Space(10);

        // Draw button to apply settings
        if (GUILayout.Button("Apply Spring Settings"))
            controller.ApplySpringSettings();
    }
}
