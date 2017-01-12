using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
	public static int score = 0;
    public static int hightScore = 0;
	public TextMesh display;
    public TextMesh display2;

    private void Start()
    {
        if (System.IO.File.Exists("save.txt"))
        {
            hightScore = int.Parse(System.IO.File.ReadAllText("save.txt"));
        }
        else
        {
            hightScore = 0;
            System.IO.File.WriteAllText("save.txt", hightScore.ToString());           
        }
    }

    void Update()
	{
		if(display)
		{
			display.text = score.ToString("D8");
		}
        if (display2)
        {
            display2.text = hightScore.ToString("D8");
        }
        if (score > hightScore)
        {
            hightScore = score;
            System.IO.File.WriteAllText("save.txt", hightScore.ToString());
        }
	}
}
