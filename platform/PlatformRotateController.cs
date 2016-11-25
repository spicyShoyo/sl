using UnityEngine;
using System.Collections;

public class PlatformRotateController : MonoBehaviour {
    public bool rotateX = false;
    public bool rotateY = false;
    public bool rotateZ = true;
    public bool flip = false;

    private const float rotateDuration = 2.0f;
    private float rotateTimer = 0;

    private const float holdDuration = 2.0f;
    private float holdTimer = 0;

    private int reverse = 1;    //1 for forward, -1 for reverse
    private bool rotating = false;

    private Vector3 size;
    private Vector3 rotatePoint;
    private Vector3 rotateAxis;

    private float rotateAngle = 90.0f;
    private float angleDone = 0.0f;
    // Use this for initialization

    /// <summary>
    /// set up rotation based on settings.
    /// moveXYZ checkboxes for axis
    /// flip checkbox for direction
    /// 
    /// works for cube so far,
    /// because getting rotation point depends on the scale.
    /// does not work for city model, which has scale of 1,1,1
    /// </summary>
    void Start () {
        if(rotateX)
        {
            size = gameObject.GetComponent<Renderer>().bounds.size;
            rotatePoint = new Vector3(0, 0, size.x);
            rotateAxis = transform.right;
        }else if(rotateY)
        {
            size = gameObject.GetComponent<Renderer>().bounds.size;
            rotatePoint = new Vector3(0, size.y, 0);
            rotateAxis = transform.up;
        }else
        {
            size = gameObject.GetComponent<Renderer>().bounds.size;
            rotatePoint = new Vector3(0, 0, size.z);
            rotateAxis = transform.forward;
        }

        if(flip)
        {
            rotateAxis *= -1;
        }
        
        rotating = true;
        gameObject.tag = "Finish";
    }
	
	// Update is called once per frame
    /// <summary>
    /// rotate then hold for hold duration
    /// </summary>
	void Update () {
        if(rotating)
        {
            rotate();
        }else
        {
            if (holdTimer < holdDuration)
            {
                holdTimer += Time.deltaTime;
            }
            else
            {
                rotating = true;
                holdTimer = 0;
                gameObject.tag = "Finish";
            }
        }
        
	}

    /// <summary>
    /// rotate
    /// </summary>
    void rotate()
    {
        if(rotateTimer < rotateDuration)
        {
            rotateTimer += Time.deltaTime;
            float curAngle = Time.deltaTime / rotateDuration * rotateAngle;
            transform.RotateAround(rotatePoint, rotateAxis, reverse * curAngle);
            angleDone += curAngle;
        }else
        {
            rotating = false;
            gameObject.tag = "Untagged";
            rotateTimer = 0;
            reverse *= -1;
            if (rotateAngle - angleDone != 0)
            {
                //transform.RotateAround(rotatePoint, rotateAxis, reverse * (rotateAngle - angleDone)); //will flip for some reason
            }
        }
    }
}
