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

    public AudioClip[] audioClip;
    private bool canPlay = true;

    void Update()
    {
        if (Input.GetButton(buttonName))
        {
            GetComponent<Rigidbody>().AddForceAtPosition(forceDir.normalized * force, transform.position);
            if (canPlay)
            {
                PlaySound(0);
                canPlay = false;
            }

        }
        else
        {
            GetComponent<Rigidbody>().AddForceAtPosition(forceDir.normalized * -force, transform.position);
            canPlay = true;
        }
    }

    void PlaySound(int clip)
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.6f, 1.4f);
        GetComponent<AudioSource>().clip = audioClip[clip];
        GetComponent<AudioSource>().Play();
    }

}
