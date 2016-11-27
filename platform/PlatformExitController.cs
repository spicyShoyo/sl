using UnityEngine;
using System.Collections;

public class PlatformExitController : MonoBehaviour {
    public GameObject cube;
    private GameObject player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        print(player);
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 2)
        {
            collected();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            collected();
        }
    }

    private void collected()
    {
        PlatformGameController platformGameControllerObj = GameObject.Find("GameController").GetComponent<PlatformGameController>();
        if (platformGameControllerObj)
        {
            platformGameControllerObj.win();
        }
        Destroy(gameObject);
    }
}
