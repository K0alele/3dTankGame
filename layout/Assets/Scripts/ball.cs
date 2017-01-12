using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class ball : MonoBehaviour {

    private Vector3 OriginalPos;

    [SerializeField]
    private int zLimit = -20;

    public TextMesh display;
    public int lives = 3;

    private Rigidbody rb;

    public float slowness = 10f;

    [SerializeField]
    private string buttonName = "Fire1";

    public AudioClip[] audioClip;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        OriginalPos = rb.position;
        display.text = "BALLS: " + lives.ToString();
    }
	

	void Update ()
    {
        if (rb.position.z <= zLimit)
        {
            if (lives > 0)
            {
                rb.position = OriginalPos;
                PlaySound(0, true);
            }           
            gameObject.GetComponent<Renderer>().material.color = Color.white;
            rb.velocity = Vector3.zero;
            lives--;

            display.text = "BALLS: " + lives.ToString();            
        }
        if (lives < 0)
        {
            display.text = "GAME OVER";
            PlaySound(1,false);
            if (Input.GetButton(buttonName))
            {
                ScoreManager.score = 0;                
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
            //StartCoroutine(RestartLevel());
        }
    }

    void PlaySound(int clip, bool random)
    {
        if (random)        
            GetComponent<AudioSource>().pitch = Random.Range(0.6f, 1.4f);
        else        
            GetComponent<AudioSource>().pitch = 1f;        
        GetComponent<AudioSource>().clip = audioClip[clip];
        GetComponent<AudioSource>().Play();
    }

    IEnumerator RestartLevel()
    {
        Time.timeScale = 1f / slowness;
        Time.fixedDeltaTime = Time.fixedDeltaTime / slowness;

        yield return new WaitForSeconds(1f / slowness);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.fixedDeltaTime * slowness;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
