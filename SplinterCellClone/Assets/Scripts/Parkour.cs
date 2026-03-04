using UnityEngine;

public class Parkour : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rigidBody;
    public PlayerController plrController;
    public LayerMask wallLayerMask;

    [Header("SystemVariables")]
    public float climbSpeed;
    public float maxClimbTime;
    public float climbTimer;

    public bool isClimbing;


    [Header("Detection")]
    public float detectWallLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool isFrontWall;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (isClimbing) ClimbingMovement();
    }

    private void StateMachine()
    {
        if (isFrontWall && Input.GetButton("Climb") && wallLookAngle < maxWallLookAngle)
        {
            if (!isClimbing && climbTimer > 0)
            {
                StartClimbing();
            }

            if (climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }

            if (climbTimer < 0) StopClimb();
        }

        else 
        {
            if (isClimbing) StopClimb(); 
        }
    }
    private void WallCheck()
    {
        isFrontWall = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectWallLength, wallLayerMask);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if (plrController.isGrounded())
        {
            climbTimer = maxClimbTime;
        }
    }



    private void StartClimbing()
    {
        isClimbing = true;
    
    }


    private void ClimbingMovement()
    {
        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, climbSpeed, rigidBody.linearVelocity.z);
    
    }

    private void StopClimb()
    { 
        isClimbing = false;
    
    }
}
