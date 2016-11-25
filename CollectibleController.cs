using UnityEngine;
using System.Collections;

public class CollectibleController : MonoBehaviour {
    public GameObject cube;
    private GameObject player;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        print(player);
	}
	
	// Update is called once per frame
	void Update () {
        cube.transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        if(Vector3.Distance(player.transform.position, gameObject.transform.position) < 5)
        {
            collected();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            collected();
        }
    }

    private void collected()
    {
        CityGameController cityGameControllerObj = GameObject.Find("GameController").GetComponent<CityGameController>();
        if(cityGameControllerObj)
        {
            cityGameControllerObj.addScore();
        }
        PlatformGameController platformGameControllerObj = GameObject.Find("GameController").GetComponent<PlatformGameController>();
        if(platformGameControllerObj)
        {
            platformGameControllerObj.addScore();
        }
        Destroy(gameObject);
    }
}
