using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collision collision;

    [Header("Movement Stats")]
    public float moveSpeed = 10f; // max speed
    public float acceleration = 50f;

    [Space]
    [Header("Jump Stats")]
    public float jumpForce = 13f;
    public float coyoteTime = 0.1f; // grace period for jumping after going over ledge
    public float jumpBufferTime = 0.15f; // grace period for jump input before landing
    public float maxJumpTime = 0.2f; // maax time jump is influenced by holding button
    private float lastGroundedTime; // track time since last grounded
    private float lastJumpInputTime; // track time since jump input
    private float jumpStartTime; // track when jump started

    [Space]
    [Header("Dashing Attributes")]
    public float dashSpeed = 65f;
    public float dashAirDrag = 14f;

    [Space]
    [Header("Gravity Attributes")]
    public float holdJumpGravity = 1f; // less gravity while holding jump (for higher jump)
    public float baseGravity = 6f; // stronger gravity when falling
    private const float ZERO_GRAVITY = 0f;


    [Space]
    [Header("Status booleans")]
    private bool canMove = true;
    private bool isGrabbing;
    private bool isSliding;
    private bool wallJumped;
    private bool isJumping;
    private bool isDashing;
    private bool hasDashed;

    [Space]
    [Header("Movement Input")]
    private float horizontalMove;
    private float verticalMove;


    [Header("Wall properties")]
    public float slideSpeed = 5f;
    public float climbSpeed = 6f;
    public float wallForce = 8f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<Collision>();

        rb.gravityScale = baseGravity; // default gravity
    }

    private void Update()
    {
        horizontalMove = InputManager.GetHorizontal();
        verticalMove = InputManager.GetVertical();

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

        if (collision.isGrounded && !isDashing)
        {
            lastGroundedTime = Time.time;
            isJumping = false;
            wallJumped = false;
            hasDashed = false;
        }

        if (isGrabbing && !isDashing)
        {
            WallGrab();
        }
        else
        {
            rb.gravityScale = baseGravity;
        }

        if (collision.isWalled && !collision.isGrounded)
        {
            // if the player isn't moving and wall grabbing
            if (horizontalMove != 0 && !isGrabbing)
            {
                isSliding = true;
                WallSlide();
            }
            // if the player is jumping from a wall
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

        if (InputManager.GetDash() && !hasDashed)
        {
            Debug.Log("Testing");
            Dash(horizontalMove, verticalMove);
        }

        CheckGrounded();
    }

    private void WallSlide()
    {
        if (!canMove)
            return;

        float pushForce;

        // prevents unwanted horizontal motion that could cancel out upward force from the wall jump.
        if ((rb.linearVelocity.x > 0 && collision.rightWalled) || (rb.linearVelocity.x < 0 && collision.leftWalled))
        {
            pushForce = 0; // no push when sliding down
        }
        else
        {
            pushForce = rb.linearVelocity.x; // preserve momentum otherwise
        }
        if (isSliding /* && !collision.isGrounded */)
            rb.linearVelocity = new Vector2(pushForce, -slideSpeed);
    }

    private void WallGrab()
    {
        rb.gravityScale = ZERO_GRAVITY;

        // Prevents character from keeping y momentum while grabbing
        // if the player pushes against the wall while grabbing, we want to remove any unwanted downard momentum.
        if (!wallJumped)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalMove * climbSpeed);
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

        if (rb.linearVelocity.y > 0 && InputManager.GetJumpHeld() && Time.time - jumpStartTime <= maxJumpTime && !isDashing)
        {
            rb.gravityScale = holdJumpGravity;
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

    private void WallJump()
    {
        // reset grabbing and sliding
        isGrabbing = false;
        isSliding = false;

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(0.2f));

        Vector2 wallDir = collision.rightWalled ? Vector2.left : Vector2.right;
        Vector2 jumpDir = new Vector2(wallDir.x * wallForce, wallForce);

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = jumpDir;
        jumpStartTime = Time.time;

        // sets wall Jumped to true then false after .2s
        StartCoroutine(WallJumpingTime(0.3f));
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

        if (Mathf.Abs(horizontalMove) > 0.01f && !isDashing)
        {
            newVelocity.x = Mathf.MoveTowards(newVelocity.x, horizontalMove * moveSpeed, acceleration * Time.fixedDeltaTime);
        }
        else if (wallJumped)
        {
            newVelocity = Vector2.Lerp(newVelocity, new Vector2(horizontalMove * moveSpeed, newVelocity.y), .5f * Time.deltaTime);
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

    IEnumerator WallJumpingTime(float time)
    {
        wallJumped = true;
        yield return new WaitForSeconds(time);
        wallJumped = false;
    }

    IEnumerator DashTime(float time)
    {
        wallJumped = true;
        isDashing = true;

        // zero gravity for the dash to be linear
        rb.gravityScale = ZERO_GRAVITY;
        // air drag for the dash to feel fast but not long (distance)
        rb.linearDamping = dashAirDrag;

        yield return new WaitForSeconds(time);

        // set everything back to base after dash timing
        rb.gravityScale = baseGravity;
        rb.linearDamping = 0;
        wallJumped = false;
        isDashing = false;
    }

    private void Dash(float xDir, float yDir)
    {
        hasDashed = true;

        rb.linearVelocity = Vector2.zero;
        Vector2 dir = new Vector2(xDir, yDir);
        Debug.Log(dir);
        // set Dashing Velocity
        rb.linearVelocity += dir.normalized * dashSpeed;
        StartCoroutine(DashTime(0.3f));
    }

}