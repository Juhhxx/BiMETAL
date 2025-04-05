using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpringController))]
public class SpringControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpringController controller = (SpringController)target;

        EditorGUILayout.Space(10);

        // Draw button to apply settings
        if (GUILayout.Button("Apply Spring Settings"))
            controller.ApplySpringSettings();
    }
}
