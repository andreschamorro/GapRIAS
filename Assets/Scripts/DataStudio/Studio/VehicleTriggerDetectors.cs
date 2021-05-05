using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleTriggerDetectors : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            transform.parent.parent.GetComponent<DataStudioManager>().OnChildTriggerEnter(other, this.name);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            transform.parent.parent.GetComponent<DataStudioManager>().OnChildTriggerExit(other, this.name);
        }
    }
}

