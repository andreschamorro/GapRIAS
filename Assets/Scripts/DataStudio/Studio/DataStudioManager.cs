using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class DataStudioManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField]
    public string ID = "";
    [SerializeField]
    public VehicleSpawner vehicleSpawner;
    [SerializeField]
    public PedestrianDataRecorder pedestrianRecorder;
    [SerializeField]
    public VRDataRecorder vrRecorder;
    [SerializeField]
    public VehicleDataRecorder vehiclesRecorder;
    [SerializeField]
    public int numReplication;

    public bool disableIfNotPlaying = true;

    //interface via inspector
    [HideInInspector]
    public bool doRecord = false;
    [HideInInspector]
    public bool doCalibration = false;
    [HideInInspector]
    public bool doExperiment = false;
    [HideInInspector]
    public bool doSave = false;
    [HideInInspector]
    public bool doCancel = false;

    //private members
    // Instantiate random number generator.  
    private readonly System.Random _random = new System.Random();

    private bool isCalibrationStarted = false;
    private bool isExperimentStarted = false;
    private bool isPaused = false;

    private IEnumerator _spawnCoroutine;
    private List<Trial> _trials;
    private Trial _currentTrial;
    private int _trialExpStart; // Trials On Experiments Index
    private float[] _speeds;
    private int _currentTrialIndex = 0;

    private bool _passStart = false;
    private bool _passEnter = false;
    private bool _passGoal = false;

    private DisplayController _displayController;
    #endregion
    
    #region PROPERTIES
    public bool IsRecording { get { return IsRecordingStarted && !isPaused; } }
    public bool IsPaused { get { return isPaused; } }
    public bool IsRecordingStarted { get { return (isCalibrationStarted || isExperimentStarted); } }
    public List<Trial> Trials { get { return _trials; }}
    public void ClearTrials() { _trials.Clear(); }
    public int trialExpStart { get { return _trialExpStart; }}
    #endregion

    #region UNITY_METHODS
    // Start is called before the first frame update
    void Start()
    {
        _trials = new List<Trial>();
        _currentTrial = new Trial();
        _displayController = this.transform.Find("WorldDisplay").gameObject.GetComponent<DisplayController>();

        _speeds = new float[numReplication * 2];
        for (int i = 0; i < numReplication; i++)
        {
            _speeds[i] = 15.0f;
        }

        for (int i = numReplication; i < 2*numReplication; i++)
        {
            _speeds[i] = 25.0f;
        }
        Reshuffle(_speeds);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCalibrationStarted && doCalibration)
        {
            StartCalibration();
        }

        if (!isExperimentStarted && doExperiment)
        {
            StartExperiment();
        }

        if (doSave)
        {
            if (doCalibration)
            {
                SaveCalibration();
            }
            else if (doExperiment)
            {
                SaveExperiment();
            }
            _displayController.Trial(0);
            isPaused = doCancel = doSave = false;
        }
        else if (doCancel)
        {
            if (doCalibration)
            {
                CancelCalibration();
            }
            else if (doExperiment)
            {
                CancelExperiment();
            }
            _displayController.Trial(0);
            isPaused = doCancel = doSave = false;
        }
    }

    public void OnChildTriggerEnter(Collider other, string name)
    {
        if ((name == "Goal") && (_passStart && _passEnter))
        {
            if (pedestrianRecorder.recorder.IsRecording)
            {
                _currentTrial.PedClearCarTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PedXatClearCar = other.transform.position.x;
                _currentTrial.PedZatClearCar = other.transform.position.z;

                if (_currentTrial.PedClearCarTime > _currentTrial.PedEnterRoadwayTime)
                {
                    double Peddistance = Math.Sqrt(Math.Pow(_currentTrial.PedXatClearCar-_currentTrial.PedXatEnterRoadway,2)
                                        + Math.Pow(_currentTrial.PedZatClearCar-_currentTrial.PedZatEnterRoadway,2));
                    double Pedtime = _currentTrial.PedClearCarTime - _currentTrial.PedEnterRoadwayTime;

                    _currentTrial.PedAvgSpeed = Peddistance/Pedtime;
                    _currentTrial.VehicleSpeed = vehicleSpawner.maxVelocity;
                    _trials.Add(new Trial(_currentTrial));
                    _currentTrial.Clear();

                    _displayController.Trial(_trials.Count);
                }
            }

            if (doExperiment)
            {
                float tailPos = vehicleSpawner.activeObject.Where(go => (go.transform.position.x < other.transform.position.x)).Max(go => go.transform.position.x);
                vehicleSpawner.CleanSpawners(go => go.transform.position.x < tailPos);
                vehicleSpawner.minVelocity = vehicleSpawner.maxVelocity = _speeds[_currentTrialIndex++];
                if (_currentTrialIndex == numReplication * 2)
                {
                    _currentTrialIndex = 0;
                }
            }

            _passGoal = _passEnter = false;
        }
        
        if ((name == "Start") && doCalibration && (_passGoal && _passEnter))
        {
            if (pedestrianRecorder.recorder.IsRecording)
            {
                _currentTrial.PedClearCarTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PedXatClearCar = other.transform.position.x;
                _currentTrial.PedZatClearCar = other.transform.position.z;

                if (_currentTrial.PedClearCarTime > _currentTrial.PedEnterRoadwayTime)
                {
                    double Peddistance = Math.Sqrt(Math.Pow(_currentTrial.PedXatClearCar-_currentTrial.PedXatEnterRoadway,2)
                                        + Math.Pow(_currentTrial.PedZatClearCar-_currentTrial.PedZatEnterRoadway,2));
                    double Pedtime = _currentTrial.PedClearCarTime - _currentTrial.PedEnterRoadwayTime;

                    _currentTrial.PedAvgSpeed = Peddistance/Pedtime;
                    _trials.Add(new Trial(_currentTrial));
                    _currentTrial.Clear();

                    _displayController.Trial(_trials.Count);
                }
            }

            _passGoal = _passEnter = false;
        }
        if (name == "VehiclePassthrough")
        {
            _currentTrial.Gaps.Add(other.transform.GetComponent<VehicleRIAS>().GapSee);
            Debug.Log("Name " + name + " Gap Seen " + _currentTrial.Gaps.LastOrDefault());
        }
    }
    
    public void OnChildTriggerExit(Collider other, string name)
    {
        if (name == "Start")
        {
            _passStart = true;
            if (pedestrianRecorder.recorder.IsRecording)
            {
                _currentTrial.PedEnterRoadwayTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PedXatEnterRoadway = other.transform.position.x;
                _currentTrial.PedZatEnterRoadway = other.transform.position.z;
            }
        }

        if (name == "Enter")
        {
            _passEnter = true;
        } 
        
        if (name == "Goal")
        {
            _passGoal = true;
            if (pedestrianRecorder.recorder.IsRecording)
            {
                _currentTrial.PedEnterRoadwayTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PedXatEnterRoadway = other.transform.position.x;
                _currentTrial.PedZatEnterRoadway = other.transform.position.z;
            }
        }
    }
    #endregion
    #region PUBLIC_METHODS
    public void StartCalibration()
    {
        Debug.Log("Calibration Init");
        pedestrianRecorder.recorder.recordingDirectory = "/" + ID + "/Calibration";
        pedestrianRecorder.recorder.doRecord = true;

        isCalibrationStarted = true;
    }
    public void CancelCalibration()
    {
        pedestrianRecorder.recorder.doCancel = true;
        doCalibration = isCalibrationStarted = false;
    }
    public void SaveCalibration()
    {
        pedestrianRecorder.recorder.doSave = true;
        doCalibration = isCalibrationStarted = false;
    }
    public void StartExperiment()
    {
        pedestrianRecorder.recorder.recordingDirectory = "/" + ID + "/Experiment";
        vehiclesRecorder.recorder.recordingDirectory = "/" + ID + "/Experiment";
        vrRecorder.recorder.recordingDirectory = "/" + ID + "/Experiment";

        pedestrianRecorder.recorder.doRecord = true;
        vrRecorder.recorder.doRecord = true;
        vehiclesRecorder.recorder.doRecord = true;

        _trialExpStart = _trials.Count;
        isExperimentStarted = true;
        StartSpawners();
    }
    public void PauseExperiment()
    {
        StopSpawners();

        pedestrianRecorder.recorder.PauseRecording();
        vrRecorder.recorder.PauseRecording();
        vehiclesRecorder.recorder.PauseRecording();
    }

    public void CancelExperiment()
    {
        StopSpawners();

        pedestrianRecorder.recorder.doCancel = true;
        vrRecorder.recorder.doCancel = true;
        vehiclesRecorder.recorder.doCancel = true;

        doExperiment = isExperimentStarted = false;
    }

    public void SaveExperiment()
    {
        pedestrianRecorder.recorder.doSave = true;
        vrRecorder.recorder.doSave = true;
        vehiclesRecorder.recorder.doSave = true;
        doExperiment = isExperimentStarted = false;
    }
    public void Hit()
    {
        _displayController.Hit();
        _currentTrial.HitNum++;
    }
    #endregion
    #region PRIVATE_METHODS
    private void StartSpawners()
    {
        vehicleSpawner.minVelocity = vehicleSpawner.maxVelocity = _speeds[_currentTrialIndex++];
        _spawnCoroutine = vehicleSpawner.WaitAndSpawn();
        StartCoroutine(_spawnCoroutine);
    }

    private void StopSpawners()
    {
        StopCoroutine(_spawnCoroutine);
    }
    void Reshuffle(float[] array)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < array.Length; t++)
        {
            float tmp = array[t];
            int r = UnityEngine.Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }
    }
    #endregion
}
