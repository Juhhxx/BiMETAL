using UnityEditor;
using UnityEngine;
using System.IO;

public class ExtractAndRemapAnimations : EditorWindow
{
    [MenuItem("Tools/Extract + Remap Spring Animation Paths")]
    static void ExtractAndRemap()
    {
        var selected = Selection.activeObject;
        if (selected == null)
        {
            Debug.LogWarning("Please select an FBX model first.");
            return;
        }

        string fbxPath = AssetDatabase.GetAssetPath(selected);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);

        string outputDir = Path.GetDirectoryName(fbxPath) + "/ExtractedAnimations";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        int remapCount = 0;

        foreach (var obj in assets)
        {
            if (obj is AnimationClip clip && AssetDatabase.IsSubAsset(obj))
            {
                string clipName = clip.name;
                string outputPath = Path.Combine(outputDir, clipName + ".anim");

                // Duplicate the clip
                AnimationClip newClip = Object.Instantiate(clip);
                AssetDatabase.CreateAsset(newClip, outputPath);

                // Now remap paths
                SerializedObject so = new SerializedObject(newClip);
                SerializedProperty curves = so.FindProperty("m_EditorCurves");

                for (int i = 0; i < curves.arraySize; i++)
                {
                    SerializedProperty curve = curves.GetArrayElementAtIndex(i);
                    SerializedProperty pathProp = curve.FindPropertyRelative("path");

                    string oldPath = pathProp.stringValue;
                    if (oldPath.StartsWith("Spring_") && !oldPath.Contains("/"))
                    {
                        string newPath;

                        int dot = oldPath.LastIndexOf('.');
                        if (dot != -1 && dot < oldPath.Length - 1 && char.IsDigit(oldPath[dot + 1]))
                        {
                            // Has .xxx suffix
                            string suffix = oldPath.Substring(dot + 1);
                            newPath = $"{oldPath}/Icosphere.{suffix}";
                        }
                        else
                        {
                            // No suffix
                            newPath = $"{oldPath}/Icosphere";
                        }

                        pathProp.stringValue = newPath;
                        Debug.Log($"Remapped: {oldPath} → {newPath} in clip {clipName}");
                        remapCount++;
                    }
                }

                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(newClip);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Done! Extracted and remapped {remapCount} paths.");
    }
}
