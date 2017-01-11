using UnityEngine;
using System.Collections;

public class Bumper : MonoBehaviour {

    [SerializeField]
	private float force = 100.0f;
    [SerializeField]
    private float radius = 1.0f;
	
	void OnCollisionEnter(Collision col)
	{
        if (col.gameObject.GetComponent<Rigidbody>() != null)
        {            
            col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius);
            Debug.Log("COLIDDEDDEDEDEDED!!");
        }
    }
}
