using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ControlGravity : MonoBehaviour {

    // Game Objects Passed in Through Unity
    public GameObject World;
    public Camera playerCamera;
    public GameObject player;

    // Stores the current gravity
    private Vector3 curGravity;
    // Stores previous rotation values
    private Vector3 lastRotationAxis;
    private float lastRotationAngle;

    // Not sure what these 2 are for, TODO: DOCUMENT
    private float timer;
    private bool hasRotated;


	// Use this for initialization
	void Start () {
        // Default gravity is down
        curGravity = Vector3.down;

        // Setting initial values
        lastRotationAxis = Vector3.zero;
        lastRotationAngle = 0;

        timer = 0;
        hasRotated = true;

	}
	
	// Update is called once per frame
	void Update () {
        // Update Player Body and View based off input
        moveCamera();
        movePlayer();

        // Update Gravity if p is hit
	    if ((Input.GetKeyDown("p") || isBButtonPressed()) && hasRotated) { updateGravityDirection(); }
	}

    /**************************************************************************************************************/
    /*********************************************** JOYSTICK INPUT ***********************************************/
    /**************************************************************************************************************/

    /// <summary>
    /// moves the camera based off Xbox controller Right joystick movement
    /// Hope: FPS Game type Camera Control
    /// Current Implementation: Bumpers 
    /// </summary>
    void moveCamera()
    {
        
        float speed = 3.0f;
        float xRot = speed * Input.GetAxis("RightJoystickX");
        float yRot = speed * Input.GetAxis("RightJoystickY");

        transform.Rotate(xRot, yRot, 0);
        //transform.localEulerAngles = new Vector3(-yRot, transform.localEulerAngles.y, 0);
        
        if (Input.GetKeyDown("q") || isLeftBumperPressed())
        {
            transform.RotateAround(transform.position, curGravity, 2);
        }
        if (Input.GetKeyDown("e") || isRightBumperPressed())
        {
            transform.RotateAround(transform.position, curGravity, -2);
        }
        
    }

    /// <summary>
    /// moves the player based off User Input
    /// </summary>
    void movePlayer()
    {
        // Gets the Player's Rigibody to apply physics
        Rigidbody playerBody = player.GetComponent<Rigidbody>();
        
        // If the player body is in the air, keep falling.
        if (!isPlayerOnTheGround())
        {
            playerBody.velocity += 9.8f * curGravity;
        }

        // Get Movement values from keyboard & Joystick
        float moveX = CrossPlatformInputManager.GetAxis("Horizontal");
        float moveY = CrossPlatformInputManager.GetAxis("Vertical");

        // Get Directional Movement Vectors
        // TODO: This has to change to support any rotated surface
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
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

        // Get Player Movement and Check for Collision
        Vector3 horizontalVelocity = right * moveX;
        Vector3 verticalVelocity = forward * moveY;

        if (!isPlayerColliding((horizontalVelocity).normalized))
        {
            //playerBody.velocity += horizontalVelocity * 5;
        }

        if (!isPlayerColliding((verticalVelocity).normalized))
        {
            //playerBody.velocity += verticalVelocity * 5;
        }

        // Update Player Position with updated velocity
        transform.position += playerBody.velocity * 0.01f;
    }

    /// <summary>
    /// Checks to see if the player body is on a surface
    /// </summary>
    /// <returns> bool value, True if player is on ground, false if not</returns>
    bool isPlayerOnTheGround()
    {
        return Physics.Raycast(transform.position, curGravity, 1.3f + 0.1f);
    }

    /// <summary>
    /// Checks if Player is colliding with an object in the given direction
    /// </summary>
    /// <param name="direction"> Normalized Vector3 representing the direction to check for collision </param>
    /// <returns> bool </returns>
    bool isPlayerColliding(Vector3 direction)
    {
        return Physics.Raycast(transform.position, direction, 2.1f);
    }

    /********************************************************************************************************************/
    /*********************************************** GRAVITY MANIPULATION ***********************************************/
    /********************************************************************************************************************/

    /// <summary>
    /// Sets the gravity of the player in the direction the player is looking. Gravity can only vary among the 6 primary axis
    /// </summary>
    void updateGravityDirection()
    {
        curGravity = getNewGravityDirection();
        
        // TODO: Add in rotating to reset user camera
    }

    /// <summary>
    /// Uses RaycastHit to get the first object in the players line of sight, then iterates upward through GameObject Tree to find the wall it is bound to.
    /// The walls represent the 6 primary axis.
    /// </summary>
    /// <returns> Normalized Vector3 of the new gravity direction </returns>
    Vector3 getNewGravityDirection()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(playerCamera.transform.position, transform.forward, 100.0F);

        GameObject wall = hits[0].transform.gameObject;
        while (wall.tag != "Wall")
        {
            wall = wall.transform.parent.gameObject;
        }

        return Vector3.Normalize(wall.transform.position);
    }

    /*********************************************************************************************************************/
    /*********************************************** MISC HELPER FUNCTIONS ***********************************************/
    /*********************************************************************************************************************/
    bool isBButtonPressed()
    {
        return Input.GetAxis("B") != 0 ? true : false;
    }

    bool isLeftBumperPressed()
    {
        return Input.GetAxis("LeftBumper") != 0 ? true : false;
    }

    bool isRightBumperPressed()
    {
        return Input.GetAxis("RightBumper") != 0 ? true : false;
    }
}
