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
    private bool isGrounded;
    private bool isJumping;
    private float lastGroundedTime; // track time since last grounded
    private float lastJumpInputTime; // track time since jump input
    private float jumpStartTime; // track when jump started

    [Header("Raycast Settings")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 5f; // default gravity
    }

    private void Update()
    {
        moveInput = InputManager.GetMoveInput();
        CheckGrounded();

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
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            isJumping = false; // Reset jumping state
        }

        bool canJump = (Time.time - lastGroundedTime <= coyoteTime) || isGrounded; // check within coyotetime
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
        else // Falling
        {
            rb.gravityScale = fallGravityMultiplier;
        }
    }

    /// <summary>
    /// Executes the jump by setting vertical velocity and tracking jump start time
    /// </summary>
    private void Jump() //execute jump
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
        jumpStartTime = Time.time; // Record jump start time
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            isJumping = false; // Reset jumping state when grounded
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}