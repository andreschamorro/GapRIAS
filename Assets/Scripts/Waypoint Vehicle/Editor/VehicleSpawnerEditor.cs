using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(VehicleSpawner), true)]
public class VehicleSpawnerEditor : Editor
{
    SerializedProperty waypointLap;
    SerializedProperty prefab;
    SerializedProperty minSpawnTime;
    SerializedProperty maxSpawnTime;
    SerializedProperty minVelocity;
    SerializedProperty maxVelocity;

    GUIStyle buttonStyle;
    GUILayoutOption height;

    void OnEnable()
    {
        VehicleSpawner spawner = target as VehicleSpawner;

        // Setup the SerializedProperties.
        waypointLap = serializedObject.FindProperty("_waypointLap");
        prefab = serializedObject.FindProperty("_prefab");
        minSpawnTime = serializedObject.FindProperty("_minSpawnTime");
        maxSpawnTime = serializedObject.FindProperty("_maxSpawnTime");
        minVelocity = serializedObject.FindProperty("_minVelocity");
        maxVelocity = serializedObject.FindProperty("_maxVelocity");
    }
    public override void OnInspectorGUI()
    {
        VehicleSpawner spawner = target as VehicleSpawner;
        serializedObject.Update();

        buttonStyle = EditorStyles.miniButtonMid;
        height = GUILayout.Height(20);
        DrawDefaultInspector();
        EditorGUILayout.Space();

        PropertyFieldMinMaxSlider("Gap", minSpawnTime, maxSpawnTime, 0, 10, height);
        PropertyFieldMinMaxSlider("Velocity", minVelocity, maxVelocity, 0, 50, height);        

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }
    private void PropertyFieldMinMaxSlider(string label, SerializedProperty min, SerializedProperty max, 
                                            float minLimit, float maxLimit, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(height);
        GUILayout.Label(label);
        float minval = min.floatValue, maxval = max.floatValue;
        minval = EditorGUILayout.FloatField(minval, GUILayout.Width(30));
        EditorGUILayout.MinMaxSlider(ref minval, ref maxval, minLimit, maxLimit);
        maxval = EditorGUILayout.FloatField(maxval, GUILayout.Width(30));
        min.floatValue = minval;
        max.floatValue = maxval;
        GUILayout.EndHorizontal();
    }
}
