using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ball : MonoBehaviour {

    private Vector3 OriginalPos;

    [SerializeField]
    private int zLimit = -20;

    private Rigidbody rb;

	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        OriginalPos = rb.position;
	}
	

	void Update ()
    {
        if (rb.position.z <= zLimit)
        {
            rb.position = OriginalPos;
            rb.velocity = Vector3.zero;
        }
	}
}
