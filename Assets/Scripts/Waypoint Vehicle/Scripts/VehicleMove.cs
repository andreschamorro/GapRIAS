using System;
using UnityEngine;

public class VehicleMove : MonoBehaviour
{
    #region FIELDS

    [Header("Settings")]
    [SerializeField]
    [Range(1.0f, 100.0f)]
    private float _speedMove = 10.0f;

    [SerializeField]
    [Range(1.0f, 100.0f)]
    private float _speedRotate = 50.0f;

    [SerializeField]
    private Vector3 _offset = new Vector3(0.0f, 0.5f, 0.0f);

    [SerializeField]
    private float _rayCastDistance = 1.0f;

    private float _hitVehicleDistance = Mathf.Infinity;

    private bool _collitionStop = false;
    private bool _externalStop = false;
    private Vector3 _position;
    private Quaternion _rotation;

    public event Action OnEndOfRoad;

    #endregion

    #region PROPERTIES

    public bool CollitionStop   // property
    {
        get { return (_collitionStop || (_hitVehicleDistance < _rayCastDistance)); }
        set { _collitionStop = value; }
    }

    public bool ExternalStop   // property
    {
        get { return _externalStop; }
        set { _externalStop = value; }
    }

    // Speed values are received in MPH thus the need to multiply 
    // by 1.4666667 for conversion into m/s.
    public float SpeedMPH   // property
    {
        set { _speedMove = value * 0.44704f; }
        get { return _speedMove / 0.44704f; }
    }

    public float Speed   // property m/s
    {
        set { _speedMove = value; }
        get { return _speedMove; }
    }

    private Vector3 Position
    {
        get
        {
            _position = transform.position - _offset;
            return _position;
        }

        set
        {
            _position = value;
            transform.position = _position + _offset;
        }
    }

    private Quaternion Rotation
    {
        get
        {
            _rotation = transform.rotation;
            return _rotation;
        }

        set
        {
            _rotation = value;
            transform.rotation = _rotation;
        }
    }

    #endregion

    #region UNITY_METHODS

    private void Update()
    {
        UpdateRayCast();
    }

    private void OnTriggerExit(Collider other)
    {
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * (_rayCastDistance + transform.localScale.z));
    }

    #endregion

    #region PUBLIC_METHODS

    public void Move(Vector3 currentPoint)
    {
        if (CollitionStop || ExternalStop) return;

        var targetLookRotation = currentPoint - Position;

        if (targetLookRotation != Vector3.zero)
        {
            Rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(currentPoint - Position, Vector3.up),
                _speedRotate * Time.deltaTime);
        }

        Position = Vector3.MoveTowards(
            Position,
            currentPoint,
            _speedMove * Time.deltaTime);

        if (Vector3.Distance(Position, currentPoint) <= 0.5f)
        {
            OnEndOfRoad?.Invoke();
        }
    }

    #endregion

    #region PRIVATE_METHODS

    private void UpdateRayCast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Vehicle"))
            {
                _hitVehicleDistance = hit.distance;
            }
        }        
    }

    #endregion
}