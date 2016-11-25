using UnityEngine;
using System.Collections;

public class PlatformGameController : MonoBehaviour {
    public AudioSource collectAudio;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void addScore()
    {
        collectAudio.Play();
    }
}
