using UnityEngine;
using System.Collections;

public class PlatformMoveController : MonoBehaviour {
    public bool moveX = false;
    public bool moveY = false;
    public bool moveZ = true;
    public bool flip = false;
    public int setDistance = 0;

    private const float moveDuration = 2.0f;
    private float moveTimer = 0;

    private const float holdDuration = 2.0f;
    private float holdTimer = 0;

    private int reverse = 1;    //1 for forward, -1 for reverse
    private bool moving = false;

    private Vector3 size;
    private Vector3 moveDir;

    private float moveDistance = 0;
    private float distanceDone = 0.0f;
    // Use this for initialization

    /// <summary>
    /// same as platformrotatecontroller
    /// is setdistance no input
    /// use the scale size
    /// </summary>
    void Start()
    {
        if (moveX)
        {
            size = gameObject.GetComponent<Renderer>().bounds.size;
            moveDir = transform.right;
            moveDistance = size.x;
        }
        else if (moveY)
        {
            size = gameObject.GetComponent<Renderer>().bounds.size;
            moveDir = transform.up;
            moveDistance = size.y;
        }
        else
        {
            size = gameObject.GetComponent<Renderer>().bounds.size;
            moveDir = transform.forward;
            moveDistance = size.z;
        }

        if (flip)
        {
            moveDir *= -1;
        }

        if(setDistance != 0)
        {
            moveDistance = setDistance;
        }

        moving = true;
        gameObject.tag = "Finish";
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            move();
        }
        else
        {
            if (holdTimer < holdDuration)
            {
                holdTimer += Time.deltaTime;
            }
            else
            {
                moving = true;
                holdTimer = 0;
                gameObject.tag = "Finish";
            }
        }

    }

    void move()
    {
        if (moveTimer < moveDuration)
        {
            moveTimer += Time.deltaTime;
            float curDistance = Time.deltaTime / moveDuration * moveDistance;
            transform.position += reverse * curDistance * moveDir;
            distanceDone += curDistance;
        }
        else
        {
            moving = false;
            gameObject.tag = "Untagged";
            moveTimer = 0;
            reverse *= -1;
            if (moveDistance - distanceDone != 0)
            {
                //transform.position += reverse * (moveDistance - distanceDone) * moveDir;
            }
        }
    }
}
