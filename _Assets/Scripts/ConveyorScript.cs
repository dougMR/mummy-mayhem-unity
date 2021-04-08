using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorScript : MonoBehaviour
{
    public float speed = 16;
    private float _maxVelocity = 16f;

    // While touching an object
    void OnTriggerStay(Collider other)
    {
        // Debug.Log("Trigger Conveyor");
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if ( rb != null || rb.velocity.magnitude <= _maxVelocity )
            rb.AddForce(transform.forward * speed);
    }
}
