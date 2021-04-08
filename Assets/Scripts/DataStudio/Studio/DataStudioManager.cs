using System;
using System.Collections;
using System.Collections.Generic;
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
    public VehicleDataRecorder vehiclesRecorder;

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
    private bool isCalibrationStarted = false;
    private bool isExperimentStarted = false;
    private bool isPaused = false;

    private IEnumerator _spawnCoroutine;
    private List<Trial> _trials;
    private Trial _currentTrial;
    private int _trialExpStart; // Trials On Experiments Index

    private bool _passStart = false;
    private bool _passEnter = false;
    private bool _passGoal = false;
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
            isPaused = doCancel = doSave = false;
        }
    }

    public void OnChildTriggerEnter(Collider other, string name)
    {
        if ((name == "Goal") && (_passStart && _passEnter))
        {
            if (pedestrianRecorder.recorder.IsRecording)
            {
                _currentTrial.PetClearCarTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PetXatClearCar = other.transform.position.x;
                _currentTrial.PetZatClearCar = other.transform.position.z;

                if (_currentTrial.PetClearCarTime > _currentTrial.PetEnterRoadwayTime)
                {
                    double Petdistance = (Math.Pow(_currentTrial.PetXatClearCar-_currentTrial.PetXatEnterRoadway,2)
                                        + Math.Pow(_currentTrial.PetZatClearCar-_currentTrial.PetZatEnterRoadway,2));
                    double Pettime = _currentTrial.PetClearCarTime - _currentTrial.PetEnterRoadwayTime;

                    _currentTrial.PetAvgSpeed = (float)(Petdistance/Pettime);
                    _trials.Add(new Trial(_currentTrial));
                }
            }

            _passGoal = _passEnter = false;
        }
        
        if ((name == "Start") && doCalibration && (_passGoal && _passEnter))
        {
            if (pedestrianRecorder.recorder.IsRecording)
            {
                _currentTrial.PetClearCarTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PetXatClearCar = other.transform.position.x;
                _currentTrial.PetZatClearCar = other.transform.position.z;

                if (_currentTrial.PetClearCarTime > _currentTrial.PetEnterRoadwayTime)
                {
                    double Petdistance = (Math.Pow(_currentTrial.PetXatClearCar-_currentTrial.PetXatEnterRoadway,2)
                                        + Math.Pow(_currentTrial.PetZatClearCar-_currentTrial.PetZatEnterRoadway,2));
                    double Pettime = _currentTrial.PetClearCarTime - _currentTrial.PetEnterRoadwayTime;

                    _currentTrial.PetAvgSpeed = (float)(Petdistance/Pettime);
                    _trials.Add(new Trial(_currentTrial));
                }
            }

            _passGoal = _passEnter = false;
        }
    }
    
    public void OnChildTriggerExit(Collider other, string name)
    {
        if (name == "Start")
        {
            _passStart = true;
            if (pedestrianRecorder.recorder.IsRecording)
            {
                _currentTrial.PetEnterRoadwayTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PetXatEnterRoadway = other.transform.position.x;
                _currentTrial.PetZatEnterRoadway = other.transform.position.z;
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
                _currentTrial.PetEnterRoadwayTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PetXatEnterRoadway = other.transform.position.x;
                _currentTrial.PetZatEnterRoadway = other.transform.position.z;
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

        pedestrianRecorder.recorder.doRecord = true;
        vehiclesRecorder.recorder.doRecord = true;

        _trialExpStart = _trials.Count;
        isExperimentStarted = true;
        StartSpawners();
    }
    public void PauseExperiment()
    {
        StopSpawners();

        pedestrianRecorder.recorder.PauseRecording();
        vehiclesRecorder.recorder.PauseRecording();
    }

    public void CancelExperiment()
    {
        StopSpawners();

        pedestrianRecorder.recorder.doCancel = true;
        vehiclesRecorder.recorder.doCancel = true;

        doExperiment = isExperimentStarted = false;
    }

    public void SaveExperiment()
    {
        pedestrianRecorder.recorder.doSave = true;
        vehiclesRecorder.recorder.doSave = true;
        doExperiment = isExperimentStarted = false;
    }
    #endregion
    #region PRIVATE_METHODS
    private void StartSpawners()
    {
        _spawnCoroutine = vehicleSpawner.WaitAndSpawn();
        StartCoroutine(_spawnCoroutine);
    }

    private void StopSpawners()
    {
        StopCoroutine(_spawnCoroutine);
    }
    #endregion
}
