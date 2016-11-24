using UnityEngine;
using System.Collections;

public class BendCityController : MonoBehaviour {
    public bool up = false;
    public bool right = true;
    public bool forward = false;
    public bool flip = false;
    public float rotateAngle = 90.0f;

    private GameObject anchor;
    private GameObject[] childCityObjList;
    private Vector3 rotateAxis;
    private Vector3 rotatePoint;

    private const float rotateDuration = 4.0f;
    private float rotateTimer = 0;
    private float angleDone = 0.0f;
    private int reverse = 1;
    private bool rotating = false;
    private bool recursive = false;

    private const int childCityIdx0 = 3;
    // Use this for initialization
    void Start () {
        
        if (transform.childCount > childCityIdx0)
        {
            childCityObjList = new GameObject[(transform.childCount - childCityIdx0)];

            for (int i= childCityIdx0; i<transform.childCount; i++)
            {
                childCityObjList[i- childCityIdx0] = transform.GetChild(i).gameObject;
            }
            
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(rotating)
        {
            rotate();
        }
	}

    public void rotateBlock(bool rec)
    {
        anchor = transform.GetChild(1).gameObject;  //can't put to start for some reason
        recursive = rec;
        if (flip)
        {
            reverse = -1;
        }
        rotateAxis = anchor.transform.right;
        if (up)
        {
            rotateAxis = anchor.transform.up;
        }
        if (forward)
        {
            rotateAxis = anchor.transform.forward;
        }

        rotatePoint = anchor.transform.position;
        rotating = true;
    }

    private void rotate()
    {
        if (rotateTimer < rotateDuration)
        {
            rotateTimer += Time.deltaTime;
            float curAngle = Time.deltaTime / rotateDuration * rotateAngle;
            transform.RotateAround(rotatePoint, rotateAxis, reverse * curAngle);
            angleDone += curAngle;
        }
        else
        {
            rotating = false;
            gameObject.tag = "Untagged";
            rotateTimer = 0;
            reverse *= -1;
            if (rotateAngle - angleDone != 0)
            {
                //transform.RotateAround(rotatePoint, rotateAxis, reverse * (rotateAngle - angleDone)); //will flip for some reason
            }
            if (transform.childCount > childCityIdx0 && recursive)
            {
                for(int i=0; i<childCityObjList.Length; i++)
                {
                    childCityObjList[i].GetComponent<BendCityController>().rotateBlock(recursive);
                }
            }
        }
    }
}
