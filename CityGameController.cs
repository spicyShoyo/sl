using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CityGameController : MonoBehaviour
{
    public GameObject firstCityObj;
    public AudioSource collectAudio;
    public AudioSource powerAudio;
    public GameObject[] cityObjList;
    private int counter = 0;
    public Text menuText;
    public Text scoreText;
    private int score;
    private int platformLeft;
    private bool gameIsOver = false;
    // Use this for initialization
    void Start()
    {
        firstCityObj.GetComponent<BendCityController>().rotateBlock(true);
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            scoreText.text = "";
            gameOverMenu();
            if (Input.GetKeyDown("joystick button 0"))
            {
                SceneManager.LoadScene("City", LoadSceneMode.Single);
            }
            if (Input.GetKeyDown("joystick button 1"))
            {
                SceneManager.LoadScene("Mainmenu", LoadSceneMode.Single);
            }
        }
        else
        {
            scoreText.text = "Power: " + score.ToString();
            if (Input.GetKeyDown("joystick button 2"))
            {
                rotateCity();
            }
        }
    }

    public void addScore()
    {
        score += 10;
        collectAudio.Play();
    }

    private void rotateCity()
    {
        if (score >= 10)
        {
            if (counter < cityObjList.Length)
            {
                cityObjList[counter].GetComponent<BendCityController>().rotateBlock(false);
                score -= 10;
                counter += 1;
                powerAudio.Play();
                if (counter == cityObjList.Length)
                {
                    gameIsOver = true;
                }
            }
            else
            {
                gameIsOver = true;
            }

        }
    }

    private void gameOverMenu()
    {
        string optionText = "Mission Complete!\nPress A:\nrestart\nPress B: mainmenu\n";
        menuText.text = optionText;
    }
}
