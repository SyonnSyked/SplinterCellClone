using UnityEngine;

public class WallJump : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] Transform orientation;

    [SerializeField] float wallCheckDistance = 1f;
    [SerializeField] float jumpUpForce = 8f;
    [SerializeField] float jumpSideForce = 6f;

    [SerializeField] LayerMask whatIsWall;

    bool wallRight;
    bool wallLeft;

    RaycastHit rightWallHit;
    RaycastHit leftWallHit;


    // Update is called once per frame
    void Update()
    {
        CheckForWall();

        if ((wallLeft || wallRight) && Input.GetKeyDown(KeyCode.Space))
        {
            WallJumpMove();
        }
    }


    void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);

        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }


    void WallJumpMove()
    {
        Vector3 jumpForce = transform.up * jumpUpForce;

        if (wallRight)
            jumpForce += -orientation.right * jumpSideForce;

        if (wallLeft)
            jumpForce += orientation.right * jumpSideForce;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(jumpForce, ForceMode.Impulse);
    }

}