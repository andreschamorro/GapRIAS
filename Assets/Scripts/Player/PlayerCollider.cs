using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class PlayerCollider : MonoBehaviour
{
    public UnityEvent ActionOnTriggerCollision;

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Vehicle"))
        {
            ActionOnTriggerCollision.Invoke();
        }
    }
}
