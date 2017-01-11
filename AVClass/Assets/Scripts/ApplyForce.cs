using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ApplyForce : MonoBehaviour {

    [SerializeField]
    private float force = 100.0f;
    [SerializeField]
    private string buttonName = "Fire1";
    [SerializeField]
    private Vector3 forceDir = Vector3.forward;

    void Update()
    {
        if (Input.GetButton(buttonName))      
            GetComponent<Rigidbody>().AddForceAtPosition(forceDir.normalized * force, transform.position);
        else GetComponent<Rigidbody>().AddForceAtPosition(forceDir.normalized * -force, transform.position);      
    }
}
