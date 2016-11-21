using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour {
    private readonly float distToGround = 0.5f;
    private readonly float randForce = 0.5f;
    private readonly float frictionCoefficient = 0.05f;
    private readonly float minVelocity = 0.08f;
    private readonly float gravityConstant = 9.8f;

    public Vector3 curGravity;
    private Rigidbody body;

    private float randForce1;
    private float randForce2;

    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody>();
        curGravity = new Vector3(1, 0, 0);

        randForce1 = Random.Range(-randForce, randForce);
        randForce2 = Random.Range(-randForce, randForce);
    }
	
	// Update is called once per frame
	void Update () {
        if(!checkGround())
        {
            applyGravity();
        }
        else
        {
            applyFriction();
        }
    }

    private bool checkGround()
    {
        return Physics.Raycast(transform.position, curGravity, distToGround);
    }

    private void applyGravity()
    {
        Vector3 curForce = curGravity;
        if (curGravity.x != 0)
        {
            curForce += new Vector3(0, randForce1, randForce2);
        }
        else if (curGravity.z != 0)
        {
            curForce += new Vector3(randForce1, randForce2, 0);
        }
        else
        {
            curForce += new Vector3(randForce1, 0, randForce2);
        }
        body.AddForce(gravityConstant * curForce, ForceMode.Acceleration);
    }

    private void applyFriction()
    {
        if(body.velocity.magnitude < minVelocity)
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
        else
        {
            Vector3 curForce = new Vector3(-frictionCoefficient * Mathf.Sign(body.velocity.x), -frictionCoefficient * Mathf.Sign(body.velocity.y), -frictionCoefficient * Mathf.Sign(body.velocity.z));
            body.AddForce(gravityConstant * curForce, ForceMode.Acceleration);
        }
    }

    public void setGravity(Vector3 newGravity)
    {
        curGravity = newGravity;
        randForce1 = Random.Range(-randForce, randForce);
        randForce2 = Random.Range(-randForce, randForce);
    }
}
