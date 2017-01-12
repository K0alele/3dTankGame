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
                {
                    Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                    Vector3 myVector = Vector3.forward;
                    Vector3 rotateVector = rotation * myVector;
                    Debug.Log(rotateVector);
                    ball.AddForce(rotateVector.normalized * Random.Range(forceMin, forceMax), ForceMode.VelocityChange);
                }                              
        }
        else if (buttonName == "none" && !velocityConstraint)
        {
            foreach (Rigidbody ball in list)
            {
                Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                Vector3 myVector = Vector3.forward;
                Vector3 rotateVector = rotation * myVector;
                Debug.Log(rotateVector);
                ball.AddForce(rotateVector.normalized * Random.Range(forceMin, forceMax), ForceMode.VelocityChange);
            }
               
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