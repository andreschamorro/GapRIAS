using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagLight : MonoBehaviour
{
    private List<GameObject> vehiclesInIntersection;
    private bool flagInRed = true;
    // Start is called before the first frame update
    void Start()
    {
        vehiclesInIntersection = new List<GameObject>();
        InvokeRepeating("SwitchFlagLight", 20, 20);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SwitchFlagLight()
    {
        flagInRed = !flagInRed;
        //Wait few seconds after light transition before making the other car move
        Invoke("MoveVehiclesQueue", 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerStop(other.gameObject);
    }

    void TriggerStop(GameObject vehicle)
    {
        vehicle.GetComponent<ObjectMove>().ExternalStop = true;
        vehiclesInIntersection.Add(vehicle);
    }

    void OnTriggerExit(Collider other) {
        ExitStop(other.gameObject);
    }

    void ExitStop(GameObject vehicle){
        vehicle.GetComponent<ObjectMove>().ExternalStop = false;
        vehiclesInIntersection.Remove(vehicle);
    }

    void MoveVehiclesQueue(){
        //Move all vehicles in queue
        List<GameObject> nVehiclesQueue = new List<GameObject>(vehiclesInIntersection);
        foreach(GameObject vehicle in vehiclesInIntersection){
            vehicle.GetComponent<ObjectMove>().ExternalStop = false;
            nVehiclesQueue.Remove(vehicle);
        }
        vehiclesInIntersection = nVehiclesQueue;
    }
}
