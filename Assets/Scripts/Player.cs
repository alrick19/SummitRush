using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f; // max speed
    public float acceleration = 50f;

    private bool canMove = true;

    [Header("Jump Settings")]
    public float jumpForce = 13f;
    public float holdJumpGravity = 1f; // less gravity while holding jump (for higher jump)
    public float baseGravity = 6f; // stronger gravity when falling
    public float coyoteTime = 0.1f; // grace period for jumping after going over ledge
    public float jumpBufferTime = 0.15f; // grace period for jump input before landing
    public float maxJumpTime = 0.2f; // maax time jump is influenced by holding button

    private const float ZERO_GRAVITY = 0f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isJumping;
    private float lastGroundedTime; // track time since last grounded
    private float lastJumpInputTime; // track time since jump input
    private float jumpStartTime; // track when jump started

    private Collision collision;


    [Header("Wall properties")]
    public float slideSpeed = 5f;
    public float wallForce = 8f;

    private bool isGrabbing;
    private bool isSliding;
    private bool wallJumped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<Collision>();

        rb.gravityScale = baseGravity; // default gravity
    }

    private void Update()
    {
        moveInput = InputManager.GetMoveInput();

        StateChecks();
    }

    private void StateChecks()
    {
        HandleMovement();
        // check wall grabbing
        if (collision.isWalled && InputManager.GetWallGrab())
        {
            isGrabbing = true;
            isSliding = false;
        }
        if (!InputManager.GetWallGrab() || !collision.isWalled)
        {
            isGrabbing = false;
            isSliding = false;
        }

        if (collision.isGrounded)
        {
            lastGroundedTime = Time.time;
            isJumping = false;
            wallJumped = false;
        }

        if (isGrabbing)
        {
            Debug.Log("Zero gravity");
            rb.gravityScale = ZERO_GRAVITY;
        }
        else
        {
            rb.gravityScale = baseGravity;
        }

        if (collision.isWalled && !collision.isGrounded)
        {
            // if the player isn't moving and wall grabbing
            if (InputManager.GetMoveInput() != 0 && !isGrabbing)
            {
                isSliding = true;
                WallSlide();
            }
            if (InputManager.GetJumpInput())
            {
                WallJump();
                lastJumpInputTime = 0;
            }
        }

        if (!collision.isWalled && collision.isGrounded)
        {
            isSliding = false;
        }

        if (InputManager.GetJumpInput())
        {
            lastJumpInputTime = Time.time;
        }
        BetterJump();

        CheckGrounded();
    }

    private void WallSlide()
    {
        if (isSliding /* && !collision.isGrounded */)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slideSpeed);
    }

    private void BetterJump()
    {
        bool canJump = (Time.time - lastGroundedTime <= coyoteTime) || collision.isGrounded; // check within coyotetime
        bool bufferedJump = Time.time - lastJumpInputTime <= jumpBufferTime; // check for within buffer time

        if (bufferedJump && canJump && !isJumping)
        {
            if (collision.isGrounded)
            {
                Jump(); // jump handled within grace periods
                lastJumpInputTime = 0; // Reset jump buffer
            }
        }

        if (rb.linearVelocity.y > 0 && InputManager.GetJumpHeld() && Time.time - jumpStartTime <= maxJumpTime)
        {
            rb.gravityScale = holdJumpGravity;
        }
        // else
        // {
        //     rb.gravityScale = baseGravity;
        // }

    }

    /// <summary>
    /// Executes the jump by settig vertical velocity and tracking jump start time
    /// </summary>
    private void Jump() //execute jump
    {
        Debug.Log("Jumping");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
        jumpStartTime = Time.time; // Record jump start time
    }

    private void WallJump()
    {
        // reset grabbing and sliding
        isGrabbing = false;
        isSliding = false;

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(0.2f));

        Vector2 wallDir = collision.rightWalled ? Vector2.left : Vector2.right;
        Vector2 jumpDir = (Vector2.up + wallDir) * wallForce;

        rb.linearVelocity = new Vector2(jumpDir.x, wallForce);
        jumpStartTime = Time.time;

        wallJumped = true;
    }

    /// <summary>
    /// Controls horizontal movement using linear velocity
    /// </summary>
    private void HandleMovement()
    {
        if (!canMove)
            return;

        if (isGrabbing)
            return;

        Vector2 newVelocity = rb.linearVelocity;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            newVelocity.x = Mathf.MoveTowards(newVelocity.x, moveInput * moveSpeed, acceleration * Time.fixedDeltaTime);
        }
        else if (wallJumped)
        {
            newVelocity = Vector2.Lerp(newVelocity, new Vector2(moveInput * moveSpeed, newVelocity.y), .5f * Time.deltaTime);
        }
        else // NO SLIDING
        {
            newVelocity.x = 0;
        }

        rb.linearVelocity = newVelocity;
    }

    /// <summary>
    /// Checks if the player is currently touching the ground every frame.
    /// An overlap circle at the player's feet check for any collision with the ground layer
    /// </summary>
    private void CheckGrounded()
    {
        if (collision.isGrounded && rb.linearVelocity.y <= 0)
        {
            isJumping = false; // Reset jumping state when grounded
        }
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

}