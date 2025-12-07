using UnityEngine;
using UnityEditor;

// This script only works in the Editor, not in a build.
public class RecalculateBoundsEditor : EditorWindow
{
    [MenuItem("Tools/Recalculate Selected Bounds")]
    public static void RecalcBounds()
    {
        // Get the currently selected GameObject
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogWarning("No GameObject selected. Please select the Floating Head.");
            return;
        }

        // Get the MeshFilter component
        MeshFilter meshFilter = selectedObject.GetComponent<MeshFilter>();

        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            // The core command to recalculate the bounds!
            meshFilter.sharedMesh.RecalculateBounds();
            Debug.Log($"Bounds recalculated for: {selectedObject.name}");
        }
        else
        {
            Debug.LogError($"Could not find MeshFilter or Mesh on {selectedObject.name}.");
        }
    }
}