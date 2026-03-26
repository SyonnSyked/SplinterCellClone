using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AdvancedMovement : MonoBehaviour
{
    [Header("PlayerInput")]
    [SerializeField] PlayerInputComponent playerInput;

    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float climbSpeed;
    public float vaultSpeed;
    public float airMinSpeed;

    [SerializeField] StaminaComponent staminaComponent;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.2f;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    public Climbing climbingScript;

    public Transform orientation;

    [Header("~~~~~~Audio~~~~~")]
    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;

    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;

    [SerializeField] AudioClip[] audStep;
    [SerializeField] float audStepVol;

    [Header("~~~~~~Step Timing~~~~~")]
    [SerializeField] float walkStepInterval = 0.5f;
    [SerializeField] float sprintStepInterval = 0.3f;


    bool isPlayingStep;
    bool isSprinting;

    Vector2 plrInput;
    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        wallrunning,
        climbing,
        vaulting,
        crouching,
        sliding,
        air
    }

    public bool usingStamina;
    public bool sprinting;
    public bool sliding;
    public bool crouching;
    public bool wallrunning;
    public bool climbing;
    public bool vaulting;

    public bool freeze;
    public bool unlimited;

    public bool restricted;

    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;

    private void Start()
    {
        climbingScript = GetComponent<Climbing>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {


        //Debug.Log(state.ToString());
        //Debug.Log(restricted);
        //Debug.Log(desiredMoveSpeed);
        //Debug.Log(orientation);
        //Debug.Log($"h={horizontalInput} v={verticalInput} moveDir={moveDirection} orientation={(orientation ? orientation.name : "NULL")}");
        //Debug.Log($"exitingWall={(climbingScript ? climbingScript.exitingWall : false)}");
        //Debug.Log(OnSlope());
        //Debug.Log(wallrunning);
        // ground check

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        //grounded = Physics.SphereCast(transform.position, groundRadius, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.3f, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();
        PlayerUsingStamina();
        HandleStaminaEmpty();
        //TextStuff();

        HandleSteps();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

    }

    private void HandleSteps()
    {
        bool isMoving = rb.linearVelocity.magnitude < 0.1f; // Only step when moving on ground
        if (grounded && isMoving && !isSprinting)
        {
            if (!isPlayingStep)
            {
                StartCoroutine(PlayStep(audStep, walkStepInterval));
            }
        }
        else if (grounded && isMoving && sprinting)
        {
            if (!isPlayingStep)
            {
                StartCoroutine(PlayStep(audStep, sprintStepInterval));
            }
        }

    }

    private IEnumerator PlayStep(AudioClip[] clips, float interval)
    {
        isPlayingStep = true;

        // Play a random step sound
        if (clips != null && clips.Length > 0)
            aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        yield return new WaitForSeconds(interval);

        isPlayingStep = false;
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }



    private void MyInput()
    {
        plrInput = playerInput.groundedMove.action.ReadValue<Vector2>();
        moveDirection = orientation.forward * plrInput.y + orientation.right * plrInput.x;
        moveDirection.y = 0f;

        if (playerInput.jump.action.IsPressed() && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (playerInput.sprint.action.IsPressed() && grounded)
        {
            sprinting = true;
        }

        if (!playerInput.sprint.action.IsPressed() && grounded)
        {
            sprinting = false;
        }

        if (playerInput.crouch.action.IsPressed() && moveDirection == Vector3.zero)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            crouching = true;
        }

        if (!playerInput.crouch.action.IsPressed())
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            crouching = false;
        }
    }

    bool keepMomentum;
    private void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            rb.linearVelocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        else if (unlimited)
        {
            state = MovementState.unlimited;
            desiredMoveSpeed = 999f;
        }

        else if (vaulting)
        {
            state = MovementState.vaulting;
            desiredMoveSpeed = vaultSpeed;
        }

        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
            usingStamina = true;
        }

        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
            usingStamina = true;
        }

        else if (sliding)
        {
            state = MovementState.sliding;

            // increase speed by one every second
            if (OnSlope() && rb.linearVelocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                keepMomentum = true;
            }

            else
                desiredMoveSpeed = sprintSpeed;
        }

        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
            usingStamina = false;
        }

        else if (sprinting)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            usingStamina = true;
        }

        else if (grounded && !sprinting)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            usingStamina = false;
        }

        else
        {
            state = MovementState.air;
            usingStamina = false;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (climbingScript.exitingWall) return;
        if (restricted) return;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        if (!wallrunning) rb.useGravity = !OnSlope();
    }

    private bool PlayerUsingStamina()
    {
        if (usingStamina)
        {
            staminaComponent.SetUsingStamina(true);

            return true;
        }

        staminaComponent.SetUsingStamina(false);
        return false;
    }

    private void HandleStaminaEmpty()
    {
        if (!staminaComponent.HasStamina())
        {
            DisableParkour();
        }
        else if (staminaComponent.HasStamina())
        {
            EnableParkour();
        }
    }

    private void DisableParkour()
    {
            playerInput.sprint.action.Disable();
            playerInput.climb.action.Disable();
            playerInput.wallRun.action.Disable();
    }

    private void EnableParkour()
    { 
            playerInput.sprint.action.Enable();
            playerInput.climb.action.Enable();
            playerInput.wallRun.action.Enable();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if (audJump.Length > 0)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }


}
