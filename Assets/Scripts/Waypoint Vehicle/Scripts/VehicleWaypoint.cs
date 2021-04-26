using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleMove))]
public class VehicleWaypoint : MonoBehaviour
{
    #region FIELDS

    [SerializeField]
    private WaypointLap _waypointLap = default;

    private VehicleMove _vehicleMove = default;

    [SerializeField]
    private bool _isDrive = true;

    [SerializeField]
    private bool _isWait = false;

    [SerializeField]
    private bool _isRepeat = true;

    private List<Vector3> _points;
    private Vector3 _currentPoint;
    private int _pointIndex;
    private float _wait;

    public event Action<GameObject> OnEndPath;
    public event Action<GameObject> OnRepeatPath;

    #endregion

    #region UNITY_METHODS

    private void Awake()
    {
        _wait = UnityEngine.Random.Range(1.0f, 5.0f);
    }

    private void Start()
    {
        _points = _waypointLap._curvePoints;
        _currentPoint = _points[0];
    }

    private void Update()
    {
        if (!_isDrive) return;
        if (_isWait)
        {
            WaitStart();
        }
        else
        {
            _vehicleMove.Move(_currentPoint);
        }
    }

    private void OnEnable()
    {
        _vehicleMove = this.GetComponent<VehicleMove>();
        _vehicleMove.OnEndOfRoad += GetNextPoint;
    }

    private void OnDisable()
    {
        _vehicleMove.OnEndOfRoad -= GetNextPoint;
    }

    #endregion

    #region PUBLIC_METHODS

    public WaypointLap WaypointLap   // property
    {
        get { return _waypointLap; }
        set { _waypointLap = value; }
    }

    public bool isDrive   // property
    {
        get { return _isDrive; }
        set { _isDrive = value; }
    }

    public int PointIndex   // property
    {
        get { return _pointIndex; }
        set { _pointIndex = value; }
    }

    public void GetNextPoint()
    {
        if (_waypointLap.Lap)
        {
            if (_pointIndex >= _points.Count - 1)
            {
                if (_pointIndex == _points.Count - 1)
                {
                    EndPath();
                    return;
                }
                else
                {
                    _pointIndex++;
                    _currentPoint = new Vector3(_points[0].x, 0.0f, _points[0].z);
                }
            }
            else
            {
                _pointIndex++;
                _currentPoint = new Vector3(_points[_pointIndex].x, 0.0f, _points[_pointIndex].z);
            }
        }
        else
        {
            if (_pointIndex >= _points.Count - 1)
            {
                EndPath();
                return;
            }
            else
            {
                _pointIndex++;
                _currentPoint = new Vector3(_points[_pointIndex].x, 0.0f, _points[_pointIndex].z);
            }
        }
    }

    public void EndPath()
    {
        if (_isRepeat)
        {
            _pointIndex = 0;

            OnRepeatPath?.Invoke(this.gameObject);
        }
        else
        {
            _isDrive = false;

            OnEndPath?.Invoke(this.gameObject);
        }
    }

    #endregion

    #region PRIVATE_METHODS

    private void WaitStart()
    {
        _wait -= Time.deltaTime;

        if (_wait < 0)
        {
            _isWait = false;
        }
    }
    
    private bool IsOnSameLane(Transform otherCar)
    {
        Vector3 diff = this.transform.forward - otherCar.transform.forward;
        if (Mathf.Abs(diff.x) < 0.3f && Mathf.Abs(diff.z) < 0.3f) return true;
        else return false;
    }

    #endregion
}