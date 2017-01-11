using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Launcher : MonoBehaviour
{
    [SerializeField]
    private float forceMin = 20.0f;
    [SerializeField]
    private float forceMax = 60.0f;
    [SerializeField]
    private string buttonName = "Fire1";
    [SerializeField]
    private bool velocityConstraint = false;

    //Lista de bolas no trigger
    private List<Rigidbody> list;

    void Start()
    {
        list = new List<Rigidbody>();
    }

    void Update()
    {
        if (buttonName != "none")
        {
            if (Input.GetButtonDown(buttonName))
                foreach (Rigidbody ball in list)
                    ball.AddForce(Vector3.forward * Random.Range(forceMin, forceMax), ForceMode.VelocityChange);
        }
        else if (buttonName == "none" && !velocityConstraint)
        {
            foreach (Rigidbody ball in list)
                ball.AddForce(Vector3.forward * Random.Range(forceMin, forceMax), ForceMode.VelocityChange);
        }                                     
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Rigidbody>())        
            list.Add(col.GetComponent<Rigidbody>());        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<Rigidbody>())        
            list.Remove(col.GetComponent<Rigidbody>());        
    }
}