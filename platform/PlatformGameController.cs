using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlatformGameController : MonoBehaviour {
    public AudioSource collectAudio;
    public AudioSource checkpointAudio;
    public Text socreText;
    public Text winText;
    private int score = 0;
    private bool gameOver = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        socreText.text = "Score: " + score.ToString();
        if(gameOver)
        {
            if (Input.GetKeyDown("joystick button 0"))
            {
                SceneManager.LoadScene("Platform", LoadSceneMode.Single);
            }
            if (Input.GetKeyDown("joystick button 1"))
            {
                SceneManager.LoadScene("Mainmenu", LoadSceneMode.Single);
            }
        }
    }

    public void addScore()
    {
        collectAudio.Play();
        score += 10;
    }
    
    public void win()
    {
        socreText.text = "";
        winText.text = "Final Score: " + score.ToString() + "\nPress B go back\nPress A to replay";
        gameOver = true;
    }

    public void checkpoint()
    {
        checkpointAudio.Play();
        GameObject.Find("Player").GetComponent<ParkourController>().saveState();
    }
}
