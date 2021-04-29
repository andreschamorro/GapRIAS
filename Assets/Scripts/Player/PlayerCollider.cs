using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCollider : MonoBehaviour
{
    public Transform head;
    public UnityEvent ActionOnTriggerCollision;

    private CapsuleCollider capsuleCollider;

    //-------------------------------------------------
    void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }


    //-------------------------------------------------
    void FixedUpdate()
    {
        float distanceFromFloor = Vector3.Dot(head.localPosition, Vector3.up);
        capsuleCollider.height = Mathf.Max(capsuleCollider.radius, distanceFromFloor);
        transform.localPosition = head.localPosition - 0.5f * distanceFromFloor * Vector3.up;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Vehicle"))
        {
            ActionOnTriggerCollision.Invoke();
        }
    }
}
