using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(VRRecorder))]
[ExecuteInEditMode]
public class VRDataRecorder : MonoBehaviour
{
    public Transform head;
    public Transform leftController;
    public Transform rightController;
    public VRRecorder recorder;
    // Start is called before the first frame update
    void Awake()
    {
        recorder = GetComponent<VRRecorder>();
        recorder.DefaultRecordingName = "VRTrace";
        recorder.disableIfNotPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!recorder.IsRecording)
        {
            return;
        }

        if (head == null)
        {
            Debug.LogError("<b>[SteamVR]</b> No head transform target set!t");
            return;
        }

        Vector3 scale = Vector3.one;

        VRData vrData = new VRData(head.localToWorldMatrix, leftController.localToWorldMatrix, rightController.localToWorldMatrix);
        recorder.RecordAsJson(vrData);
    }
}

