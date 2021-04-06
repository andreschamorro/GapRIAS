using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleRIAS : MonoBehaviour
{
    #region FIELDS
    private float _nextVehicleDistance = Mathf.Infinity;
    private bool canWalk = true;
    [SerializeField]
    private bool _oneLane = false;
    private Transform _pedestrianSignal;
    private float _hitVehicleDistance = Mathf.Infinity;
    private bool _hitVehicleDetect;
    Collider _vehicleCollider;
    private Vector3 _offsetCastCenter;
    private Vector3 _offsetCastExtents;
    private RaycastHit _castHit;
    #endregion

    #region PROPERTIES
    public bool CanWalk { get { return canWalk; } }
    public float GapSee { get { return _nextVehicleDistance/transform.GetComponent<VehicleMove>().Speed; } }
    public float SpeedMPH { get { return transform.GetComponent<VehicleMove>().SpeedMPH; } }
    #endregion

    #region UNITY_METHODS
    private void Awake()
    {
        _pedestrianSignal = this.transform.Find("pedestrian-signal");
        _vehicleCollider = GetComponent<Collider>();
        if (_oneLane)
        {
            _offsetCastExtents = new Vector3(3.0f, 2.0f, 0.0f);
        }
        else
        {
            _offsetCastExtents = new Vector3(6.0f, 2.0f, 0.0f);
        }
    }
    private void Start()
    {        
    }

    private void Update()
    {
        UpdateLaneCast();
    }

    private void OnTriggerExit(Collider other)
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            TriggerPedestrianSignal();
        }
    }

    private void UpdateLaneCast()
    {
        int vehicleLayerMask = 1 << LayerMask.NameToLayer("Vehicle");
        Vector3 frontCenter = _vehicleCollider.bounds.center + Vector3.Scale(_vehicleCollider.bounds.extents, transform.forward);

        if (Physics.BoxCast(frontCenter, _offsetCastExtents, 
            transform.forward, out _castHit, transform.rotation, 
            _hitVehicleDistance, vehicleLayerMask))
        {
            _hitVehicleDetect = true;
            _nextVehicleDistance = _castHit.distance;
        }
    }

    void OnDrawGizmos()
    {
        //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (_hitVehicleDetect)
        {
            Vector3 frontCenter = transform.position + Vector3.Scale(_vehicleCollider.bounds.extents, transform.forward);
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(frontCenter, transform.forward * _castHit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(frontCenter + transform.forward * _castHit.distance, transform.rotation * _offsetCastExtents);
        }

    }

    #endregion

    #region PUBLIC_METHODS
    public void ShowPedestrianSignal()
    {
        _pedestrianSignal.gameObject.SetActive(true);
    }

    public void ActivatePedestrianSignal()
    {
        _pedestrianSignal.transform.Find("Walk").gameObject.SetActive(true);
        _pedestrianSignal.transform.Find("Wait").gameObject.SetActive(false);
        canWalk = true;
    }

    public void DesactivatePedestrianSignal()
    {
        _pedestrianSignal.transform.Find("Walk").gameObject.SetActive(false);
        _pedestrianSignal.transform.Find("Wait").gameObject.SetActive(true);
        canWalk = false;
    }

    public void TriggerPedestrianSignal()
    {
        if (canWalk)
        {
            DesactivatePedestrianSignal();
            
        }
        else
        {
            ActivatePedestrianSignal();
        }
    }
    #endregion
}
