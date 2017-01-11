using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ScoreManager))]
public class Bumper : MonoBehaviour {

    [SerializeField]
	private float force = 100.0f;
    [SerializeField]
    private float radius = 1.0f;
    [SerializeField]
    private int value = 50;

    void OnCollisionEnter(Collision col)
	{
        if (col.gameObject.GetComponent<Rigidbody>() != null)
        {
            ScoreManager.score += value;
            col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius);        
        }
    }
}
