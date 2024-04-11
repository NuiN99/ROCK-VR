using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PrefabPainter
{
    static List<GameObject> placedObjects;
    public static GameObject prefab;
    public static bool randomRotation;
    public static bool randomScale;

    public static float minScale = 1f;
    public static float maxScale = 1f;

    static GameObject visualizer;

    static Quaternion rotation;
    static float scale;

    public static void Enable()
    {
        SetRandomRotation();
        SetRandomScale();

        placedObjects = new List<GameObject>();
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void Disable()
    {
        placedObjects?.Clear();
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
        visualizer.transform.localScale *= scale;
        
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
            obj.transform.localScale *= scale;
            placedObjects.Add(obj);
            
            SetRandomScale();
            SetRandomRotation();
            Selection.activeObject = obj;
        }
    }

    static void SetRandomRotation()
    {
        rotation = randomRotation ? Quaternion.Euler(0, Random.Range(0, 360), 0) : Quaternion.identity;
    }

    static void SetRandomScale()
    {
        scale = randomScale ? Random.Range(minScale, maxScale) : 1;
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
        PrefabPainter.randomScale = EditorGUILayout.Toggle("Random Scale", PrefabPainter.randomScale);
        
        if (PrefabPainter.randomScale)
        {
            Vector2 minMax = new(PrefabPainter.minScale, PrefabPainter.maxScale);
            EditorGUILayout.Vector2Field("Value", minMax);
            EditorGUILayout.MinMaxSlider("Scale", ref PrefabPainter.minScale, ref PrefabPainter.maxScale, 0.1f, 10f);
        }

        if (GUILayout.Button("Undo Last Placement"))
        {
            PrefabPainter.UndoLastAction();
        }
    }
}