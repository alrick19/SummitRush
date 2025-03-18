using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f; // max speed
    public float acceleration = 50f;

    [Header("Jump Settings")]
    public float jumpForce = 13f;
    public float holdJumpGravity = 1f; // less gravity while holding jump (for higher jump)
    public float fallGravityMultiplier = 6f; // stronger gravity when falling
    public float lowJumpGravity = 6f; // stronger gravity when jump is cut short
    public float coyoteTime = 0.1f; // grace period for jumping after going over ledge
    public float jumpBufferTime = 0.15f; // grace period for jump input before landing
    public float maxJumpTime = 0.2f; // maax time jump is influenced by holding button

    private Rigidbody2D rb;
    private float moveInput;
    private bool isJumping;
    private float lastGroundedTime; // track time since last grounded
    private float lastJumpInputTime; // track time since jump input
    private float jumpStartTime; // track when jump started

    private Collision collision;


    [Header("Wall properties")]
    public float slideSpeed = 5f;
    public float wallPush = 8f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<Collision>();

        rb.gravityScale = fallGravityMultiplier; // default gravity
    }

    private void Update()
    {
        moveInput = InputManager.GetMoveInput();
        CheckGrounded();


        HandleWalled();

        if (InputManager.GetJumpInput())
        {
            lastJumpInputTime = Time.time;
        }

        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// Controls horizontal movement using linear velocity
    /// </summary>
    private void HandleMovement()
    {
        Vector2 newVelocity = rb.linearVelocity;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            newVelocity.x = Mathf.MoveTowards(newVelocity.x, moveInput * moveSpeed, acceleration * Time.fixedDeltaTime);
        }
        else //NO SLIDING
        {
            newVelocity.x = 0;
        }

        rb.linearVelocity = newVelocity;
    }

    /// <summary>
    /// Handles jump logic, including jump buffering, coyote time, and variable jump height.
    /// </summary>
    private void HandleJump()
    {
        if (collision.isGrounded)
        {
            lastGroundedTime = Time.time;
            isJumping = false; // Reset jumping state
        }

        bool canJump = (Time.time - lastGroundedTime <= coyoteTime) || collision.isGrounded; // check within coyotetime
        bool bufferedJump = Time.time - lastJumpInputTime <= jumpBufferTime; // check for within buffer time

        if (bufferedJump && canJump && !isJumping)
        {
            Jump(); // jump handled within grace periods
            lastJumpInputTime = 0; // Reset jump buffer
        }

        // Variable Jump Height controlled by variable gravity
        if (rb.linearVelocity.y > 0) //rising
        {
            if (InputManager.GetJumpHeld() && (Time.time - jumpStartTime <= maxJumpTime))//holding jump
            {
                rb.gravityScale = holdJumpGravity;
            }
            else // low jump
            {
                rb.gravityScale = lowJumpGravity;
            }
        }
        else if (!collision.isWalled)// Falling
        {
            rb.gravityScale = fallGravityMultiplier;
        }
    }

    /// <summary>
    /// Executes the jump by settig vertical velocity and tracking jump start time
    /// </summary>
    private void Jump() //execute jump
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
        jumpStartTime = Time.time; // Record jump start time
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

    private void HandleWalled()
    {
        if (!collision.isGrounded && collision.isWalled)
        {
            if (InputManager.GetWallGrab())
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
            else
            {
                // rb.gravityScale = fallGravityMultiplier;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slideSpeed);
            }
        }
    }

    private void WallJump()
    {
        Vector2 wallDir = collision.rightWalled ? Vector2.left : Vector2.right;

    }
}