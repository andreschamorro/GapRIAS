using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.ProBuilder;
using ViveSR;
using ViveSR.anipal.Eye;

public class ExperimentManagerWindow : EditorWindow
{
    private DataStudioManager manager;
    private Experiment experimentInfo;
    GUIStyle buttonStyle;
    GUILayoutOption height;
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/ExperimentManager")]
    public static void Init()
    {
        ExperimentManagerWindow wnd = GetWindow<ExperimentManagerWindow>();
        wnd.titleContent = new GUIContent("ExperimentManager");
    }
    private void Populate()
    {
        UnityEngine.Object[] selection = FindObjectsOfType<DataStudioManager>();

        if (selection.Length > 0)
        {
            if (selection[0] == null)
                return;
            manager = selection[0] as DataStudioManager;
        }
    }
    private void OnSelectionChange() { Populate(); }
    private void OnEnable ()
    {
        if (experimentInfo == null)
        {
            experimentInfo = CreateInstance<Experiment> ();
        }
        Populate();
        Repaint();
    }

	private void OnFocus() { Populate(); Repaint();}

    
    public void OnGUI()
    {
        if (manager == null)
        {
            /* certain actions if my asset is null */
            return;
        }

        buttonStyle = EditorStyles.miniButtonMid;
        height = GUILayout.Height(20);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUILayout.Width(this.position.width), GUILayout.Height(this.position.height));

        EditorGUILayout.Space();
        // Experiment information
        ExperimentInfo();

        EditorGUILayout.Space();
        // Experiment Scene Type
        // MakeScene();

        EditorGUILayout.Space();
        // disable gui outside play mode
        if (manager.disableIfNotPlaying && !Application.isPlaying)
        {
            EditorGUILayout.HelpBox("For this Experiment recording is disabled while Application is not playing.", MessageType.Info);
            GUI.enabled = false;
        }

        // record toggle
        RecordToggle();

        // recording group
        RecordingGroup();

        if (manager.pedestrianRecorder.recorder.IsRecording)
        {
            Repaint(); // drawn 10 times per second
        }

        //only enable save/cancel if recording has been started
        GUI.enabled = GUI.enabled && manager.pedestrianRecorder.recorder.IsRecordingStarted;

        // save button
        if (GUILayout.Button("Save Recording", buttonStyle, height))
        {
            manager.doSave = true;
            if (manager.doCalibration)
            {
                SetSignalTriggers();
            }
            SaveMeasurement(); // save measurements
        }

        // cancel button
        if (GUILayout.Button("Cancel Recording", buttonStyle, height))
        {
            manager.doCancel = true;
        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        SRanipalSetting();

        GUILayout.EndScrollView();

    }

    private void ExperimentInfo()
    {
        GUILayout.Label ("Experiment Info", EditorStyles.boldLabel);
        // Starts a horizontal group
        GUILayout.BeginHorizontal(height);
        GUILayout.Label("Type");
        if (GUILayout.Button(experimentInfo.IsOneLane? "One Lane" : "Two Lane", buttonStyle))
        {
            experimentInfo.IsOneLane = !(experimentInfo.IsOneLane);
            ChangeScene();
        }
        GUILayout.EndHorizontal();
        experimentInfo.ID = EditorGUILayout.TextField ("ID", experimentInfo.ID);
        experimentInfo.Age = EditorGUILayout.IntField ("Age", experimentInfo.Age);
        experimentInfo.Gen = (Experiment.Gender)EditorGUILayout.EnumPopup ("Gender", experimentInfo.Gen);

        if (GUILayout.Button("Save"))
        {
            experimentInfo.Date = DateTime.Now;
            string assetPathAndName = CreateTextAssetInFolder(experimentInfo.ToString(),
                    manager.pedestrianRecorder.recorder.RecordingsPath + "/" + experimentInfo.ID, "ExperimentInfo");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private string CreateTextAssetInFolder(string text, string ParentFolder, string AssetName)
    {
        System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(String.Format("{0}/{1}", Application.dataPath, ParentFolder));
        dirInfo.Create();
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(String.Format("Assets/{0}/{1}.asset", ParentFolder, AssetName));
        TextAsset asset = new TextAsset(text);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        return assetPathAndName;
    }

    private void SaveMeasurement()
    {
        List<Trial> trials = manager.Trials;
        int trialExpStart = manager.trialExpStart;
        StringBuilder textcsv = new StringBuilder();
        textcsv.AppendLine("ID,Trial," + Trial.Header());
        for (int i = 0; i < trials.Count; i++)
        {
            textcsv.AppendLine(experimentInfo.ID + "," + Convert.ToString(i) + "," + trials[i].ToString());
        }

        string assetPathAndName = CreateTextInFolder(textcsv.ToString(),
                manager.pedestrianRecorder.recorder.DestinationFolder, "Measurement.csv");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        manager.ClearTrials();
    }

    private void SRanipalSetting()
    {
        if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
        {
            GUILayout.Label("SRanipal Eye", EditorStyles.boldLabel);
            if (GUILayout.Button("Set Parameter"))
            {
                EyeParameter parameter = new EyeParameter
                {
                    gaze_ray_parameter = new GazeRayParameter(),
                };
                Error error = SRanipal_Eye_API.GetEyeParameter(ref parameter);
                Debug.Log("GetEyeParameter: " + error + "\n" +
                          "sensitive_factor: " + parameter.gaze_ray_parameter.sensitive_factor);

                parameter.gaze_ray_parameter.sensitive_factor = parameter.gaze_ray_parameter.sensitive_factor == 1 ? 0.015f : 1;
                error = SRanipal_Eye_API.SetEyeParameter(parameter);
                Debug.Log("SetEyeParameter: " + error + "\n" +
                          "sensitive_factor: " + parameter.gaze_ray_parameter.sensitive_factor);
            }

            if (GUILayout.Button("Launch Calibration"))
            {
                SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero);
            }
        }
    }

    private string CreateTextInFolder(string text, string ParentFolder, string AssetName)
    {
        System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(String.Format("{0}/{1}", Application.dataPath, ParentFolder));
        dirInfo.Create();
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(String.Format("Assets/{0}/{1}", ParentFolder, AssetName));
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(assetPathAndName, true);
        writer.WriteLine(text);
        writer.Close();

        return assetPathAndName;
    }

    private void ChangeScene()
    {
        Vector3 lanespace = experimentInfo.IsOneLane? new Vector3(0.0f, 0.0f, 1.5f) : new Vector3(0.0f, 0.0f, 3.0f);
        GameObject.Find("CityBase/CityLeft").transform.localPosition = lanespace;
        GameObject.Find("CityBase/CityRight").transform.localPosition = -lanespace;
        
        GameObject mainRoad = GameObject.Find("CityBase/Roads/MainRoad");
        ProBuilderMesh[] quatRoads = mainRoad.GetComponentsInChildren<ProBuilderMesh>(true);
        foreach (Transform road in mainRoad.transform)
        {
            ScaleRoads(road.GetComponent<ProBuilderMesh>());
            road.Find("Marker").gameObject.SetActive(!experimentInfo.IsOneLane);
        }
        
        Vector3 vpspace = experimentInfo.IsOneLane? Vector3.zero : new Vector3(0.0f, 0.0f, 1.5f);
        GameObject.Find("VehicleDynamics/WaypointLap Left").transform.localPosition = vpspace;
        GameObject.Find("VehicleDynamics/WaypointLap Rigth").transform.localPosition = -vpspace;

        GameObject.Find("DataStudio/Detectors/Start").transform.localPosition = -lanespace;
        GameObject.Find("DataStudio/Detectors/Goal").transform.localPosition = lanespace;
        GameObject.Find("DataStudio/Detectors/Enter").transform.localPosition = vpspace;
    }

    private void ScaleRoads(ProBuilderMesh quat)
    {
        float nz = experimentInfo.IsOneLane? 1.5f : 3.0f;
        UnityEngine.ProBuilder.Vertex[] vertices = quat.GetVertices();
        Vector3[] newvertex = new Vector3[quat.vertexCount];
        for(int i = 0; i < quat.vertexCount; i++)
        {
            newvertex[i] = vertices[i].position;
            newvertex[i].z = System.Math.Sign(newvertex[i].z) * nz;
        }
        // Rebuild the triangle and submesh arrays, and apply vertex positions & submeshes to `MeshFilter.sharedMesh`
        quat.RebuildWithPositionsAndFaces(newvertex, quat.faces);
        // Recalculate UVs, Normals, Tangents, Collisions, then apply to Unity Mesh.
        quat.Refresh();
    }

    private void SetSignalTriggers()
    {
        List<Trial> trials = manager.Trials;
        double pedAvgSpeed = 0.0f;
        foreach (var trial in trials)
        {
            pedAvgSpeed += trial.PedAvgSpeed;
        }
        pedAvgSpeed /= trials.Count; // m/s

        if (pedAvgSpeed < 1.0e-10f) pedAvgSpeed = 0.25;

        float laneLength = experimentInfo.IsOneLane? 3.0f : 6.0f; // meters
        float trDistance = (manager.vehicleSpawner.maxVelocity*0.44704f)*(laneLength/(float)pedAvgSpeed);

        GameObject.Find("DataStudio/SignalTriggers/Trigger In").transform.localPosition = new Vector3(-trDistance, 0.0f, 0.0f);
        GameObject.Find("DataStudio/SignalTriggers/Trigger Out").transform.localPosition = new Vector3(trDistance, 0.0f, 0.0f);;
    }

    private void FindLeaves(Transform parent, List<Transform> leafArray)
     {
        if (parent.childCount == 0)
        {
            leafArray.Add(parent);
        }
        else
        {
            foreach (Transform child in parent)
            {
                FindLeaves(child, leafArray);
            }
        }
    }
    private void RecordToggle()
    {

        Color defaultColor = GUI.backgroundColor;
        string toggleLabel;

        manager.ID = experimentInfo.ID;

        if (manager.doCalibration)
        {
            GUI.backgroundColor = Color.red;
            toggleLabel = "Recording Calibration";
        }
        else
        {
            toggleLabel = manager.IsPaused ? "Continue Calibration" : "Start Calibration";
        }

        if (!manager.doExperiment)
        {
            manager.doCalibration = GUILayout.Toggle(manager.doCalibration, toggleLabel, buttonStyle, height);
        }

        if (manager.doExperiment)
        {
            GUI.backgroundColor = Color.red;
            toggleLabel = "Recording Experiment";
        }
        else
        {
            toggleLabel = manager.IsPaused ? "Continue Experiment" : "Start Experiment";
        }
        if (!manager.doCalibration)
        {
            manager.doExperiment = GUILayout.Toggle(manager.doExperiment, toggleLabel, buttonStyle, height);
        }

        //reset background color
        GUI.backgroundColor = defaultColor;
    }

    private void RecordingGroup()
    {

        string type = "-";
        string duration = "-";
        string recordCount = "0";

        if (manager.pedestrianRecorder.recorder.Recording != null)
        {
            type = manager.pedestrianRecorder.recorder.Recording.GetType().Name;
            duration = String.Format("{0:N2}", manager.pedestrianRecorder.recorder.Recording.duration);
            recordCount = manager.pedestrianRecorder.recorder.Recording.Count().ToString();
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Recording Name", manager.pedestrianRecorder.recorder.recordingName);

        EditorGUILayout.LabelField("Type", type);
        EditorGUILayout.LabelField("Duration", duration);
        EditorGUILayout.LabelField("Count", recordCount);

        EditorGUILayout.LabelField("Destination Folder", 
                String.Format("Assets/{0}", manager.pedestrianRecorder.recorder.DestinationFolder));

        EditorGUILayout.EndVertical();
    }
}