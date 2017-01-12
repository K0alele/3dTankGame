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

    public AudioClip[] audioClip;

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
                    ball.AddForce(rotateVector.normalized * Random.Range(forceMin, forceMax), ForceMode.VelocityChange);
                    PlaySound(0);
                }                              
        }
        else if (buttonName == "none")
        {
            foreach (Rigidbody ball in list)
            {
                Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                Vector3 myVector = Vector3.forward;
                Vector3 rotateVector = rotation * myVector;
                if (velocityConstraint && ball.velocity.z > 0)
                {
                    ball.AddForce(rotateVector.normalized * Random.Range(forceMin, forceMax), ForceMode.VelocityChange);
                    PlaySound(0);
                }
                else if (!velocityConstraint)
                {
                    ball.AddForce(rotateVector.normalized * Random.Range(forceMin, forceMax), ForceMode.VelocityChange);
                    PlaySound(0);
                }

                
            }
               
        }                                     
    }

    void PlaySound(int clip)
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.6f, 1.4f);
        GetComponent<AudioSource>().clip = audioClip[clip];
        GetComponent<AudioSource>().Play();
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