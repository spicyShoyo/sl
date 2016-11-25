using UnityEngine;
using System.Collections;

public class PlatformSpinAnchor : MonoBehaviour {
    public bool up = false;
    public bool right = true;
    public bool forward = false;
    public bool flip = false;
    public float rotateAngle = 360.0f;

    private GameObject anchor;
    private GameObject[] childCityObjList;
    private Vector3 rotateAxis;
    private Vector3 rotatePoint;

    private const float rotateDuration = 1.0f;
    private float rotateTimer = 0;
    private float angleDone = 0.0f;
    private int reverse = 1;
    private bool rotating = true;

    private GameObject cubeObj;

    // Use this for initialization
    void Start()
    {
        cubeObj = transform.GetChild(0).gameObject;
        anchor = transform.GetChild(1).gameObject;  //can't put to start for some reason
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
        cubeObj.tag = "Finish";
    }

    // Update is called once per frame
    void Update()
    {
        if (rotating)
        {
            rotate();
        }
        else
        {
            rotating = true;
            cubeObj.tag = "Finish";
        }
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
            cubeObj.tag = "Untagged";
            rotateTimer = 0;
            if (rotateAngle - angleDone != 0)
            {
                //transform.RotateAround(rotatePoint, rotateAxis, reverse * (rotateAngle - angleDone)); //will flip for some reason
            }
        }
    }
}
