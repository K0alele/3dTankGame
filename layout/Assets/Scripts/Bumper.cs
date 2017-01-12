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

    public AudioClip[] audioClip;

    void OnCollisionEnter(Collision col)
	{
        if (col.gameObject.GetComponent<Rigidbody>() != null)
        {
            ScoreManager.score += value;
            col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius);
            col.gameObject.GetComponent<Renderer>().material.color = Random.ColorHSV();
            PlaySound(0);
        }
    }

    void PlaySound(int clip)
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.6f, 1.4f);
        GetComponent<AudioSource>().clip = audioClip[clip];
        GetComponent<AudioSource>().Play();
    }
}
