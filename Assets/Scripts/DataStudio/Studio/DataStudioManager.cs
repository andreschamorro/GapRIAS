using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [HideInInspector]
    public bool doRecord = false;
    //interface via inspector
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
    #endregion
    
    #region PROPERTIES
    public bool IsRecording { get { return IsRecordingStarted && !isPaused; } }
    public bool IsPaused { get { return isPaused; } }
    public bool IsRecordingStarted { get { return (isCalibrationStarted || isExperimentStarted); } }
    #endregion

    #region UNITY_METHODS
    // Start is called before the first frame update
    void Start()
    {
        _trials = new List<Trial>();
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
        if (pedestrianRecorder.recorder.IsRecording)
        {
            if (name == "Left")
            {
                _currentTrial = new Trial();
                _currentTrial.PetClearCarTime = pedestrianRecorder.recorder.Recording.duration;
                _currentTrial.PetXatClearCar = other.transform.position.x;
                _currentTrial.PetZatClearCar = other.transform.position.z;

                if (_currentTrial.PetClearCarTime > _currentTrial.PetEnterRoadwayTime)
                {
                    double Petdistance = (Math.Pow(_currentTrial.PetXatClearCar-_currentTrial.PetXatEnterRoadway,2)
                                        + Math.Pow(_currentTrial.PetZatClearCar-_currentTrial.PetZatEnterRoadway,2));
                    double Pettime = _currentTrial.PetClearCarTime - _currentTrial.PetEnterRoadwayTime;

                    _currentTrial.PetAvgSpeed = (float)(Petdistance/Pettime);
                    _trials.Add(_currentTrial);
                }
            }
        }
    }
    
    public void OnChildTriggerExit(Collider other, string name)
    {
        if (pedestrianRecorder.recorder.IsRecording)
        {
            if (name == "Rigth")
            {
                _currentTrial = new Trial();
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

    public void SaveMeasurement()
    {

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
