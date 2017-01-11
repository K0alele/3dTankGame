using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Launcher : MonoBehaviour
{
    [SerializeField]
    private float force = 100.0f;
    [SerializeField]
    private string buttonName = "Fire1";

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
                    ball.AddForce(Vector3.forward * force, ForceMode.VelocityChange);
        }
        else
        {
            foreach (Rigidbody ball in list)
                ball.AddForce(Vector3.forward * force, ForceMode.VelocityChange);
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