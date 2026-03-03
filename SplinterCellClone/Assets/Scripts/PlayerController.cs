using System.Collections;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;
using HealthComponent;
using PlayerState;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
public class PlayerController : MonoBehaviour, iEntity
{
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] CharacterController characterController;
    [SerializeField] CameraController cameraController;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float climbSpeed;
    [SerializeField] float sprintModifier = 1.7f;
    [SerializeField] float crouchModifier;
    [SerializeField] HealthComponent playerHC;
    [SerializeField] int jumpVelocity = 15;
    [SerializeField] int jumpMax = 1;
    [SerializeField] float gravity = 32f;
    [SerializeField] PlayerState state; 
    [SerializeField] Transform throwPoint;
    [SerializeField] float throwForce = 20f;
    [SerializeField] int pushVelTime;

    [SerializeField] GameObject DebugGunPref;
    Gun Gun;
    [SerializeField] Transform WeaponHoldPos;

    int jumpCount = 0;
    int startingHP;
    float startingMovespeed;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVelocity;
    Vector3 pushVel;

    void Start()
    {

    }

    void Update()
    {
        Movement();
        Sprint();
    }

    void Movement()
    {
        if (characterController.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.Zero;
        }

        pushVel = Vector3.Lerp(pushVel, Vector3.Zero, pushVelTime * Time.deltTime);

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);
        characterController.Move(moveDir * moveSpeed * Time.deltTime);

        if (Input.GetButton("Jump"))
        {
            Jump();
        }

        characterController.Move((playerVelocity + pushVel) * Time.deltTime);
        playerVelocity.y -= gravity * Time.deltTime;
    }


    void Jump()
    {
        if (jumpCount < jumpMax)
        {
            playerVelocity.y = jumpVelocity;
            jumpCount++;
        }
    }


    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            moveSpeed *= sprintModifier;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            moveSpeed = startingMovespeed;
        }
    }


}


