using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityGameController : MonoBehaviour {
    public GameObject firstCityObj;
    public AudioSource collectAudio;
    public Text scoreText;
    private int score;
	// Use this for initialization
	void Start () {
        firstCityObj.GetComponent<BendCityController>().rotateBlock(true);
        score = 0;
    }
	
	// Update is called once per frame
	void Update () {
        scoreText.text = "Score: " + score.ToString();
    }

    public void addScore()
    {
        score += 1;
        collectAudio.Play();
    }
}
