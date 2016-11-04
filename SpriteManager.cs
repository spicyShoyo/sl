using UnityEngine;
using System.Collections;

public class SpriteManager : MonoBehaviour {
    public GameObject spriteSphere;
    public GameObject player;
    // Use this for initialization
    void Start () {
	
	}
	// this is more physically correct.
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("u"))
        {
            GameObject curSphere = (GameObject)Instantiate(spriteSphere, gameObject.transform);
            GameObject temp = spriteSphere;
            spriteSphere = curSphere;
            curSphere = temp;
            curSphere.transform.position = Vector3.zero;
            //curSphere.transform.position;
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);
            float z = Random.Range(-0.1f, 0.1f);
            if (x >= y && x >= z)
            {
                curSphere.GetComponent<SpriteController>().setGravity(new Vector3(Mathf.Sign(x), 0, 0));
            }
            else if (y >= x && y >= z)
            {
                curSphere.GetComponent<SpriteController>().setGravity(new Vector3(0, Mathf.Sign(y), 0));
            }
            else
            {
                curSphere.GetComponent<SpriteController>().setGravity(new Vector3(0, 0, Mathf.Sign(z)));
            }
            //print(curSphere.GetComponent<SpriteController>().curGravity);
            //curSphere.GetComponent<SpriteController>().setGravity(Vector3.zero);
        }
    }
}
