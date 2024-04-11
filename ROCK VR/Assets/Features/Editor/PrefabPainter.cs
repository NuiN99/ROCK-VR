using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PrefabPainter
{
    static List<GameObject> placedObjects;
    public static GameObject prefab;
    public static bool randomRotation;

    static GameObject visualizer;

    static Quaternion rotation;

    public static void Enable()
    {
        SetRandomRotation();

        placedObjects = new List<GameObject>();
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void Disable()
    {
        placedObjects.Clear();
        Object.DestroyImmediate(visualizer);
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if(visualizer != null) Object.DestroyImmediate(visualizer);
        
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        if (prefab == null)
        {
            return;
        }
        
        visualizer = Object.Instantiate(prefab, hit.point, rotation);
        
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Please select a prefab");
                return;
            }

            GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (obj == null) return;
            obj.transform.SetPositionAndRotation(hit.point, rotation);
            placedObjects.Add(obj);
            
            SetRandomRotation();
            Selection.activeObject = obj;
        }
    }

    static void SetRandomRotation()
    {
        rotation = randomRotation ? Quaternion.Euler(0, Random.Range(0, 360), 0) : Quaternion.identity;
    }
    
    public static void UndoLastAction()
    {
        if (placedObjects.Count <= 0) return;
        GameObject lastPlaced = placedObjects[^1];
        Undo.DestroyObjectImmediate(lastPlaced);
        placedObjects.RemoveAt(placedObjects.Count - 1);
    }
}

public class PrefabPainterWindow : EditorWindow
{
    GameObject _prefabToPlace;
    
    [MenuItem("Window/Prefab Painter")]
    public static void ShowWindow()
    {
        GetWindow<PrefabPainterWindow>("Prefab Painter");
        PrefabPainter.Enable();
    }

    void OnDestroy()
    {
        PrefabPainter.Disable();
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab Painter", EditorStyles.boldLabel);

        PrefabPainter.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab to Place", PrefabPainter.prefab, typeof(GameObject), false);
        PrefabPainter.randomRotation = EditorGUILayout.Toggle("Random Rotation", PrefabPainter.randomRotation);

        if (GUILayout.Button("Undo Last Placement"))
        {
            PrefabPainter.UndoLastAction();
        }
    }
}