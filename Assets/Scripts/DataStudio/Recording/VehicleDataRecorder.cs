using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RecordAndRepeat.Recorder))]
[ExecuteInEditMode]
public class VehicleDataRecorder : MonoBehaviour
{
    public SpawnerBase spawner;
    public RecordAndRepeat.Recorder recorder;
    // Start is called before the first frame update
    void Awake()
    {
        recorder = GetComponent<RecordAndRepeat.Recorder>();
        recorder.DefaultRecordingName = "VehicleTrace";
        recorder.disableIfNotPlaying = false;

        spawner.SpawnEvent += RecorderSpawn;
        spawner.DespawnEvent += RecorderDespawn;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!recorder.IsRecording)
        {
            return;
        }

        if (spawner == null)
        {
            Debug.LogWarning("No Spawner target set!");
            return;
        }

        foreach (var obj in spawner.activeObject)
        {
            VehicleData vehicleData = new VehicleData(obj, VehicleData.State.Moving);
            recorder.RecordAsJson(vehicleData);
        }
    }

    private void RecorderSpawn(GameObject obj)
    {
        VehicleData vehicleData = new VehicleData(obj, VehicleData.State.Creation);
        recorder.RecordAsJson(vehicleData);
    }
    private void RecorderDespawn(GameObject obj)
    {
        VehicleData vehicleData = new VehicleData(obj, VehicleData.State.Termination);
        recorder.RecordAsJson(vehicleData);
    }
}
