using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ParkourController : MonoBehaviour {
    private GameObject eye;
    private Rigidbody body;

    private Vector3 curGravity;

    private Vector3 lastRotationAxis;
    private float lastRotationAngle;

    private Vector3 rotateAxis;
    private float rotateAngle;
    private float angleDone;
    private float fallAccerlation = 9.8f;

    private float rotateTimer;
    private bool hasRotated;    //true if no gravity transition in progress
    private bool hasJumped; //true is jumping

    private const float rotateDuration = 0.5f; //2 sec
    private const float distToGround = 1.3f;  //magic number
    private const float distToWall = 2.1f; //magic number
    private const float GravityConstant = 9.8f * 1.5f;
    private float gravitySpeed = 0.0f;
    private const float AccerlationConstant = 6.0f;

    private const float jumpDuration = 1.0f;
    private float jumpTimer;

    private Vector3 jumpUpDir;
 
    private const float jumpUpSpeedInit = 10;

    // Use this for initialization
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
        eye = transform.FindChild("Camera").gameObject;

        curGravity = new Vector3(0, -1, 0);
        lastRotationAxis = new Vector3(0, 0, 0);
        lastRotationAngle = 0;

        rotateTimer = 0;
        hasRotated = true;
        hasJumped = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasJumped)
        {
            if (!checkGround())
            {
                gravitySpeed += GravityConstant * Time.deltaTime;
                body.velocity = gravitySpeed * curGravity;
            }
            else
            {
                gravitySpeed = 0;
            }
        }


        Debug.DrawLine(eye.transform.position, eye.transform.position + eye.transform.forward * 10, Color.green);
        if ((Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("p")) && hasRotated)
        {
            hasJumped = true;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(eye.transform.position, eye.transform.forward, 100.0F);
            for (int i = 0; i < hits.Length; i++)  //need to refractor
            {
                RaycastHit hit = hits[i];
                Renderer rend = hit.transform.GetComponent<Renderer>();

                Vector3 newGravity = getGravity(hit.transform.localEulerAngles);

                if (newGravity != curGravity)
                {

                    setupFall(newGravity);
                    setupRotation(curGravity, newGravity);

                    curGravity = newGravity;

                    angleDone = 0;
                    rotateTimer = 0;
                    hasRotated = false;
                }
                break;
            }
        }

        // Rotate Player camera to match new gravity direction
        if (!hasRotated) { rotateCamera(); }

        if ((Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("space")) && (hasRotated && hasJumped))
        {
            hasJumped = false;
            jumpUpDir = -curGravity;

            jumpTimer = 0;

            Vector3 normProj = Vector3.Project(eye.transform.forward, curGravity);
            gravitySpeed = -jumpUpSpeedInit;
        }

        if (!hasJumped)
        {
            jumpCamera();
        }

        if (hasRotated)   //only done then take in input from user
        {
            updateMovement();   //wasd
            updateViewDir();    // q and e
        }
    }

    private void jumpCamera()
    {
        if(jumpTimer >= jumpDuration)
        {
            hasJumped = true;
        }else
        {
            body.velocity = Vector3.zero;
            if (!checkCollision((jumpUpDir).normalized) && gravitySpeed < 0)
            {
                body.velocity += gravitySpeed * curGravity;
            }else if(!checkGround() && gravitySpeed >= 0)
            {
                body.velocity += gravitySpeed * curGravity;
            }

            gameObject.transform.position += body.velocity * 0.01f;
            float elapse = Time.deltaTime;
            jumpTimer += elapse;

            gravitySpeed += GravityConstant * elapse;
        }



    }

    /// <summary>
    /// Get the gravit of the hitted object
    /// </summary>
    /// <param name="localEular">local eular component of the hitted object</param>
    /// <returns>The gravity of the hitted object</returns>
    private Vector3 getGravity(Vector3 localEular)
    {
        //not safe enough
        float x = localEular.x;
        float y = localEular.y;
        float z = localEular.z;
        if (z > 80 && z < 100)
        {
            return new Vector3(1, 0, 0);
        }
        else if (z > 260 && z < 280)
        {
            return new Vector3(-1, 0, 0);
        }
        else if (x > 260 && x < 280)
        {
            return new Vector3(0, 0, 1);
        }
        else if (x > 80 && x < 100)
        {
            return new Vector3(0, 0, -1);
        }
        else if (z > 170 && z < 190)
        {
            return new Vector3(0, 1, 0);
        }
        else
        {
            return new Vector3(0, -1, 0);
        }
    }


    /// <summary>
    /// setup rotation for gravity change
    /// </summary>
    /// <param name="curGravity">current gravity</param>
    /// <param name="newGravity">new gravity</param>
    private void setupRotation(Vector3 curGravity, Vector3 newGravity)
    {
        Vector3 crossVec = Vector3.Cross(curGravity, newGravity).normalized;
        if (crossVec.magnitude == 0)
        {
            Vector3 eyeLeft = -eye.transform.right; //always flip upward
            Vector3 eyeForward = eye.transform.forward;
            Vector3 normProj = Vector3.Project(eyeForward, curGravity);
            rotateAxis = Vector3.Normalize((eyeForward - normProj));
            rotateAngle = 180.0f;
        }
        else
        {
            rotateAxis = crossVec;
            rotateAngle = 90.0f;
        }
    }


    /// <summary>
    /// Rotate the camera smoothly
    /// </summary>
    private void rotateCamera()
    {
        if (rotateTimer < rotateDuration)
        {
            if (checkGround())
            {   //if already on ground, rotate faster
                rotateTimer += 1.2f * Time.deltaTime;
                float curAngle = 1.2f * Time.deltaTime / rotateDuration * rotateAngle;
                transform.RotateAround(transform.position, rotateAxis, curAngle);
                angleDone += curAngle;
            }
            else
            {
                rotationFall();
                rotateTimer += Time.deltaTime;
                float curAngle = Time.deltaTime / rotateDuration * rotateAngle;
                transform.RotateAround(transform.position, rotateAxis, curAngle);
                angleDone += curAngle;
            }
        }
        else
        {
            rotationFall();
            hasRotated = true;
            rotateTimer = 0;
            if (rotateAngle - angleDone != 0)
            {
                transform.RotateAround(transform.position, rotateAxis, rotateAngle - angleDone);
            }
        }
    }


    /// <summary>
    /// get the fall distance when changing gravity
    /// </summary>
    /// <param name="newGravity">the new gravity</param>
    /// <returns></returns>
    private float getFallDistance(Vector3 newGravity)
    {
        float res = 6.8f;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, newGravity, 100.0F);
        for (int i = 0; i < hits.Length; i++)  //need to refractor
        {
            RaycastHit hit = hits[i];
            Renderer rend = hit.transform.GetComponent<Renderer>();
            float fallDistance = (transform.position - hit.point).magnitude;
            res = (2 * fallDistance) / (rotateDuration * rotateDuration);
            break;
        }
        return res;
    }


    /// <summary>
    /// setup the fall accleration when rotation for smooth
    /// </summary>
    /// <param name="newGravity">the new gravity</param>
    private void setupFall(Vector3 newGravity)
    {
        float fallDistance = getFallDistance(newGravity);
        fallAccerlation = (2 * fallDistance) / (rotateDuration * rotateDuration);
    }

    /// <summary>
    /// Update translation based on user input
    /// </summary>
    private void updateMovement()
    {
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        Vector3 forward = eye.transform.forward;
        Vector3 right = eye.transform.right;
        if (curGravity.x != 0)
        {
            forward.x = 0;
            right.x = 0;
        }
        else if (curGravity.y != 0)
        {
            forward.y = 0;
            right.y = 0;
        }
        else
        {
            forward.z = 0;
            right.z = 0;
        }


        if (!checkCollision((forward * vertical).normalized))
        {
            Vector3 verticalVelocity = forward * vertical;
            
            if(Input.GetKey("joystick button 8"))
            {
                body.velocity += verticalVelocity * AccerlationConstant;
            }else
            {
                body.velocity += verticalVelocity * AccerlationConstant * 2;
            }
        }
        if (!checkCollision((right * horizontal).normalized))
        {
            Vector3 horizontalVelocity = right * horizontal;
            body.velocity += horizontalVelocity * AccerlationConstant;
        }

        gameObject.transform.position += body.velocity * 0.01f;
    }


    /// <summary>
    /// Fall when rotate
    /// compared with updateMovement
    /// 1. forward is always pressed for better effect
    /// 2. GravityConstant is replaced (uncomment the section)
    /// </summary>
    private void rotationFall()
    {
        //if (!checkGround())
        //{
        //    body.velocity += fallAccerlation * curGravity;
        //}

        if (!checkGround())
        {
            body.velocity += GravityConstant * curGravity;
        }

        //moveforward for better effect
        float vertical = 1.0f;
        Vector3 forward = eye.transform.forward;
        if (curGravity.x != 0)
        {
            forward.x = 0;
        }
        else if (curGravity.y != 0)
        {
            forward.y = 0;
        }
        else
        {
            forward.z = 0;
        }


        if (!checkCollision((forward * vertical).normalized))
        {
            Vector3 verticalVelocity = forward * vertical;
            body.velocity += verticalVelocity * AccerlationConstant;
        }

        gameObject.transform.position += body.velocity * 0.01f;
    }


    /// <summary>
    ///  Simulates Joystick camera control(-45/45 rotates left/right)
    /// </summary>
    private void updateViewDir()
    {
        if (Input.GetKeyDown("q") || Input.GetKeyDown("joystick button 4"))
        {
            gameObject.transform.RotateAround(transform.position, curGravity, 45);
        }
        if (Input.GetKeyDown("e") || Input.GetKeyDown("joystick button 5"))
        {
            gameObject.transform.RotateAround(transform.position, curGravity, -45);
        }
    }


    /// <summary>
    /// Check the if on the ground, according to the current gravity
    /// </summary>
    /// <returns>true if on the ground, else false</returns>
    private bool checkGround()
    {
        return Physics.Raycast(transform.position, curGravity, distToGround + 0.1f);
    }


    /// <summary>
    /// Check ifabout to hit the wall moving to the dir
    /// </summary>
    /// <param name="dir">The direction to move along</param>
    /// <returns>true if about to hit the wall, else false</returns>
    private bool checkCollision(Vector3 dir)
    {
        return Physics.Raycast(transform.position, dir, distToWall);
    }
}
