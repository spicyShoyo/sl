using UnityEngine;
using System.Collections;

public class ThrowPortal : MonoBehaviour {
    public GameObject leftPortal;
    public GameObject rightPortal;
    public Camera eye;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left");
            throwPortal(leftPortal);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right");
            throwPortal(rightPortal);
        }
    }

    void throwPortal(GameObject portal)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(eye.transform.position, eye.transform.forward, 100.0F);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Quaternion hitObjectRotation = Quaternion.LookRotation(hit.normal);
            portal.transform.position = hit.point;
            portal.transform.rotation = hitObjectRotation;
            break;
        }
    }
}
