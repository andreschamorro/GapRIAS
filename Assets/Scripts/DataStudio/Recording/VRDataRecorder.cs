using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using ViveSR.anipal.Eye;

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

        GazeRay gazeRay;
        gazeRay.GazeOriginCombinedLocal = Vector3.zero;
        gazeRay.GazeDirectionCombinedLocal = Vector3.forward;
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING && SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT)
        {
            Vector3 camerapos = Camera.main.transform.position;
            gazeRay.GazeOriginCombinedLocal = camerapos;
        }
        else
        {
            if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out gazeRay.GazeOriginCombinedLocal, out gazeRay.GazeDirectionCombinedLocal)) { }
            else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out gazeRay.GazeOriginCombinedLocal, out gazeRay.GazeDirectionCombinedLocal)) { }
            else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out gazeRay.GazeOriginCombinedLocal, out gazeRay.GazeDirectionCombinedLocal)) { }
        }
        VRData vrData = new VRData(head.localToWorldMatrix, leftController.localToWorldMatrix, rightController.localToWorldMatrix, gazeRay);
        recorder.RecordAsJson(vrData);
    }
}

