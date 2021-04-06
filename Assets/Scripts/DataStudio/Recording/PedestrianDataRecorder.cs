using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PedestrianRecorder))]
[ExecuteInEditMode]
public class PedestrianDataRecorder : MonoBehaviour
{
    public Transform pedestrianTransform;
    public PedestrianRecorder recorder;
    // Start is called before the first frame update
    void Awake()
    {
        recorder = GetComponent<PedestrianRecorder>();
        recorder.DefaultRecordingName = "PedestrianTrace";
        recorder.disableIfNotPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!recorder.IsRecording)
        {
            return;
        }

        if (pedestrianTransform == null)
        {
            Debug.LogWarning("No transform target set!");
            return;
        }

        PedestrianData pedestrianData = new PedestrianData(pedestrianTransform);
        recorder.RecordAsJson(pedestrianData);
    }
}
