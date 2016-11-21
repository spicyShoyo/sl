using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TPTBController : MonoBehaviour {
    private Vector3 curGravity;
    private Vector3 lastRotationAxis;
    private float lastRotationAngle;
    public GameObject eye;
    public GameObject spriteSphere;

    //for gravity transition
    private Vector3 rotateAxis;
    private float rotateAngle;
    private float angleDone;

    private float timer;
    private float wait2s;
    private bool hasRotated;

    //for translation
    private float distToGround;
    private Rigidbody body;

    // Use this for initialization
    void Start () {
        curGravity = new Vector3(0, -1, 0);
        lastRotationAxis = new Vector3(0, 0, 0);
        lastRotationAngle = 0;
        
        timer = 0;
        wait2s = 2;
        hasRotated = true;

        distToGround = 1.3f; //magic number
        body = gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.DrawLine(eye.transform.position, eye.transform.position + eye.transform.forward * 10, Color.green);
        if (Input.GetKeyDown("p") && hasRotated)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(eye.transform.position, eye.transform.forward, 100.0F);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Renderer rend = hit.transform.GetComponent<Renderer>();
                Vector3 newGravity = getGravity(hit.transform.localEulerAngles);

                float rotationAngle = getRotationAngle(hit.transform.localEulerAngles);
                Debug.Log(rotationAngle);
                rotationAngle = minimizeRotation(rotationAngle);
                Debug.Log(rotationAngle);
                if (newGravity != curGravity)
                {
                    Quaternion quatFirst = Quaternion.AngleAxis(lastRotationAngle, lastRotationAxis);
                    Quaternion quatSecond = Quaternion.AngleAxis(-rotationAngle, hit.transform.localEulerAngles.normalized);
                    Quaternion quatAll = quatSecond * quatFirst;
                    quatAll.ToAngleAxis(out rotateAngle, out rotateAxis);

                    lastRotationAxis = hit.transform.localEulerAngles.normalized;
                    lastRotationAngle = rotationAngle;
                    curGravity = newGravity;

                    angleDone = 0;
                    timer = 0;
                    hasRotated = false;
                }
            }
        }

        // Rotate Player camera to match new gravity direction
        if (!hasRotated) { rotateCamera(); }

        if (hasRotated)   //only done then take in input from user
        {
            updateMovement();   //wasd
            updateViewDir();    // q and e
        }


        /**
         * Don't know what this does.
        if (Input.GetKeyDown("h"))
        {
            if (curCamera.fieldOfView == initFOV)
            {
                curCamera.fieldOfView = initFOV * 3 / 4;
            }
            else
            {
                curCamera.fieldOfView = initFOV;
            }
        }
        **/

        //if(Input.GetKeyDown("i"))
        //{
        //    GameObject curSphere = (GameObject)Instantiate(spriteSphere, gameObject.transform);
        //    float x = Random.Range(-0.1f, 0.1f);
        //    float y = Random.Range(-0.1f, 0.1f);
        //    float z = Random.Range(-0.1f, 0.1f);
        //    if (x >= y && x >= z)
        //    {
        //        curSphere.GetComponent<SpriteController>().setGravity(new Vector3(Mathf.Sign(x), 0, 0));
        //    }
        //    else if (y >= x && y >= z) {
        //        curSphere.GetComponent<SpriteController>().setGravity(new Vector3(0, Mathf.Sign(y), 0));
        //    }else
        //    {
        //        curSphere.GetComponent<SpriteController>().setGravity(new Vector3(0, 0, Mathf.Sign(z)));
        //    }
        //    print(curSphere.GetComponent<SpriteController>().curGravity);
        //    curSphere.GetComponent<SpriteController>().setGravity(Vector3.zero);
        //}
        
        //This is weird physically, but I think the effect is amazing.
        if (Input.GetKeyDown("i")) { spawnGravitySphere(); }
    }

    private void rotateCamera()
    {
        if (timer < wait2s)
        {
            timer += Time.deltaTime;
            float curAngle = Time.deltaTime / 2.0f * rotateAngle;
            transform.RotateAround(transform.position, rotateAxis, curAngle);
            angleDone += curAngle;
        }
        else
        {
            hasRotated = true;
            timer = 0;
            if (rotateAngle - angleDone != 0)
            {
                transform.RotateAround(transform.position, rotateAxis, rotateAngle - angleDone);
            }
        }
    }

    /**
     * Spawns a sphere that obeys the current gravity direction
    **/
    private void spawnGravitySphere()
    {
        GameObject curSphere = (GameObject)Instantiate(spriteSphere, gameObject.transform);
        GameObject temp = spriteSphere;
        spriteSphere = curSphere;
        curSphere = temp;
        curSphere.transform.position = Vector3.zero;
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
        curSphere.GetComponent<SpriteController>().setGravity(Vector3.zero);
    }

    /**
     * Minimizes the necessary rotation to minimize any possible nausea
     *  i.e. rotate -270 degrees = rotate 90 degrees. Hence rotate 90 degrees.
     * @param float rotationAngle: Represents the amount necessary to rotate the camera to fit new gravity direction (in float degrees)
    **/
    private float minimizeRotation(float rotationAngle)
    {
        float correctedAngle = rotationAngle;

        if (rotationAngle > 180f)
            correctedAngle = rotationAngle - 360f;
        else if (rotationAngle < -180f)
            correctedAngle = rotationAngle + 360f;

        return correctedAngle;
    }

    private void updateMovement()
    {
        if(!checkGround())
        {
            body.velocity += 9.8f * curGravity;
        }
        
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        Vector3 forward = eye.transform.forward;
        Vector3 right = eye.transform.right;
        if(curGravity.x != 0)
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
            body.velocity += verticalVelocity * 5;
        }
        if (!checkCollision((right * horizontal).normalized))
        {
            Vector3 horizontalVelocity = right * horizontal;
            body.velocity += horizontalVelocity * 5;
        }
        
        gameObject.transform.position += body.velocity * 0.01f;
    }

    /**
     * Simulates Joystick camera control (-45/45 rotates left/right)
    **/
    private void updateViewDir()
    {
        if(Input.GetKeyDown("q")) {
            gameObject.transform.RotateAround(transform.position, curGravity, 45);
        }
        if (Input.GetKeyDown("e"))
        {
            gameObject.transform.RotateAround(transform.position, curGravity, -45);
        }
    }

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

    private float getRotationAngle(Vector3 localEular)
    {
        if (localEular.x != 0)
        {
            return -localEular.x;
        }
        else if (localEular.y != 0)
        {
            return -localEular.y;
        }
        else if (localEular.z != 0)
        {
            return -localEular.z;
        }
        else
        {
            return 0;
        }
    }

    private bool checkGround()
    {
        return Physics.Raycast(transform.position, curGravity, distToGround + 0.1f);
    }

    private bool checkCollision(Vector3 dir)
    {
        return Physics.Raycast(transform.position, dir, 2.1f);
    }
}
