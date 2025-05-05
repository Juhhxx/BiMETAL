using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(HexagonTabletop))]
public class AssignTabletopEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexagonTabletop tabletop = (HexagonTabletop)target;

        if (GUILayout.Button("Assign Cells"))
        {
            HexagonCell[] cells = tabletop.transform.GetComponentsInChildren<HexagonCell>();
            tabletop.Cells = new();

            foreach (HexagonCell cell in cells)
            {
                cell.InitializeCell(tabletop);
                tabletop.Cells.Add(cell);
                EditorUtility.SetDirty(cell);
            }

            EditorUtility.SetDirty(tabletop);
            EditorSceneManager.MarkSceneDirty(tabletop.gameObject.scene);
        }
    }
}

#endif
