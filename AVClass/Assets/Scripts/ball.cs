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
            rb.position = OriginalPos;
            rb.velocity = Vector3.zero;
            lives--;

            display.text = "BALLS: " + lives.ToString();
        }
        if (lives < 0)
        {
            display.text = "GAME OVER";
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //StartCoroutine(RestartLevel());
        }
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
