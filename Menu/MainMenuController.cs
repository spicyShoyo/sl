using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Text mainMenuText;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 0"))
        {
            SceneManager.LoadScene("Falling", LoadSceneMode.Single);
        }
        if (Input.GetKeyDown("joystick button 2"))
        {
            SceneManager.LoadScene("City", LoadSceneMode.Single);
        }
    }
}
