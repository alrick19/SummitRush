using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collision collision;
    private AnimationScript anim;
    private DashTrail dashTrail;


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
    private float lastJumpInputTime = -999f; // track time since jump input
    private float jumpStartTime; // track when jump started

    [Space]
    [Header("Dashing Attributes")]
    public float dashSpeed = 65f;
    public float dashAirDrag = 14f;

    [Space]
    [Header("Gravity Attributes")]
    public float holdJumpGravity = 1f; // less gravity while holding jump (for higher jump)
    public float baseGravity = 6f; // stronger gravity when falling
    public float terminalVelocity = -20f;
    private const float ZERO_GRAVITY = 0f;

    [Space]
    [Header("Climb Timer")]
    public float maxClimbTime = 5f;
    public float currentClimbTime;



    [Space]
    [Header("Status booleans")]
    public bool canMove = true;
    public bool isGrabbing;
    public bool isSliding;
    private bool wallJumped;
    public bool isJumping;
    public bool isDashing;
    public bool hasDashed;

    [Space]
    [Header("Movement Input")]
    public float horizontalMove;
    public float verticalMove;


    [Header("Wall properties")]
    public float slideSpeed = 5f;
    public float climbSpeed = 6f;
    public float wallForce = 8f;

    [Space]
    [Header("Special Effects")]
    public ParticleSystem jumpParticle;
    public ParticleSystem slideParticle;
    private ParticleSystem deathParticle;


    private bool wasGroundedLastFrame = false;
    private Coroutine dashCoroutine;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<Collision>();
        anim = GetComponentInChildren<AnimationScript>();

        dashTrail = GetComponentInChildren<DashTrail>();
        if (dashTrail != null)
        {
            dashTrail.Initialize(transform, anim);
        }

        rb.gravityScale = baseGravity;
        currentClimbTime = maxClimbTime;
    }

    private void Update()
    {
        horizontalMove = InputManager.GetHorizontal();
        verticalMove = InputManager.GetVertical();

        StateChecks();
        HandleLoopSFX();
    }

    private void StateChecks()
    {
        HandleMovement();
        // check wall grabbing
        if (collision.isWalled && InputManager.GetWallGrab() && currentClimbTime > 0f)
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
            if (isGrabbing && collision.onLedge && verticalMove > 0)
            {
                StartCoroutine(ClimbLedge(collision.ledgePos));
            }
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

        if (InputManager.GetDash() && !hasDashed && !isDashing)
        {
            Dash(horizontalMove, verticalMove);
        }

        CheckGrounded();
        WallParticles();

        // Add terminal velocity
        if (rb.linearVelocity.y < terminalVelocity && !isDashing)
        {
            if (verticalMove < -0.1)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, terminalVelocity - 5f);

            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, terminalVelocity);

            }
        }

        HandleClimbTimer();
    }
    private void HandleLoopSFX()
    {
        bool isWalking = Mathf.Abs(horizontalMove) > 0.01f && collision.isGrounded && !isGrabbing && !isSliding && !isDashing;
        bool climbing = isGrabbing && verticalMove != 0;
        bool sliding = isSliding && !isGrabbing && rb.linearVelocity.y < -0.1f;
        if (isWalking) AudioManager.Instance.PlayLoop(AudioManager.Instance.walkLoop);
        else if (climbing) AudioManager.Instance.PlayLoop(AudioManager.Instance.wallClimbLoop);
        else if (sliding)
        {
            AudioManager.Instance.PlayLoop(AudioManager.Instance.wallSlideLoop);
        }
        else AudioManager.Instance.StopLoop();
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
        {

            rb.linearVelocity = new Vector2(pushForce, -slideSpeed);
        }

    }

    private void WallGrab()
    {
        rb.gravityScale = ZERO_GRAVITY;

        // Prevents character from keeping y momentum while grabbing
        // if the player pushes against the wall while grabbing, we want to remove any unwanted downard momentum.
        if (!wallJumped)
        {
            float ascentSpeed = verticalMove > 0 ? verticalMove : verticalMove * 1.5f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, ascentSpeed * climbSpeed);
        }

    }

    private void HandleClimbTimer()
    {
        if (isGrabbing && !isDashing)
        {
            if (verticalMove > 0)
                currentClimbTime -= 2 * Time.deltaTime; // climb time depletes twice as fast when climbing
            else
                currentClimbTime -= Time.deltaTime;

            if (currentClimbTime <= 0f)
            {
                isGrabbing = false;
                isSliding = true;
                currentClimbTime = 0f;
            }
        }
        else if (collision.isGrounded)
        {
            currentClimbTime = maxClimbTime;
        }
    }

    private void BetterJump()
    {
        bool canJump = (Time.time - lastGroundedTime <= coyoteTime) || collision.isGrounded; // check within coyotetime
        bool bufferedJump = Time.time - lastJumpInputTime <= jumpBufferTime; // check for within buffer time

        if (bufferedJump && canJump && !isJumping)
        {
            Jump(); // jump handled within grace periods
            lastJumpInputTime = 0; // Reset jump buffer
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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.jump);
        anim.SetTrigger("Jumping");
        jumpParticle.Play();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
        jumpStartTime = Time.time; // Record jump start time
    }

    private void WallJump()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.wallJump);
        // reset grabbing and sliding
        isGrabbing = false;
        isSliding = false;

        anim.SetTrigger("Jumping");

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

    private void WallParticles()
    {
        if (rb.linearVelocity.y < -0.1 && (isGrabbing || isSliding) && !isDashing)
        {
            Vector3 scale = slideParticle.transform.localPosition;
            float side = anim.sprite.flipX ? 1 : -1;
            scale.x = Mathf.Abs(scale.x) * side;
            slideParticle.transform.localPosition = scale;

            if (!slideParticle.isPlaying)
                slideParticle.Play();
        }
        else
        {
            if (slideParticle.isPlaying)
                slideParticle.Stop();
        }
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
        bool justLanded = !wasGroundedLastFrame && collision.isGrounded && rb.linearVelocity.y <= 0;

        if (justLanded)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.land);
        }

        wasGroundedLastFrame = collision.isGrounded;

        if (collision.isGrounded && rb.linearVelocity.y <= 0)
        {
            isJumping = false;
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

    IEnumerator ClimbLedge(Vector2 ledgePosition)
    {
        canMove = false;

        Vector2 startPos = transform.position;

        // Slight offset to pull player up and in
        Vector2 climbOffset = new Vector2(collision.leftWalled ? -0.25f : 0.25f, 0.5f);
        Vector2 targetPos = ledgePosition + climbOffset;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        canMove = true;
    }

    IEnumerator DashTime(float time)
    {
        if (dashTrail != null)
            dashTrail.ShowTrail();

        wallJumped = true;
        isDashing = true;

        rb.gravityScale = ZERO_GRAVITY;
        rb.linearDamping = dashAirDrag;

        yield return new WaitForSeconds(time);

        rb.gravityScale = baseGravity;
        rb.linearDamping = 0;
        wallJumped = false;
        isDashing = false;
    }

    private void Dash(float xDir, float yDir)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.dash);
        CameraShake.Shake(0.2f, 5f);
        hasDashed = true;
        anim.SetTrigger("Dash");

        rb.linearVelocity = Vector2.zero;

        if (xDir == 0 && yDir == 0)
        {
            xDir = anim.sprite.flipX ? 1 : -1;
        }

        Vector2 dir = new Vector2(xDir, yDir);

        // set Dashing Velocity
        rb.linearVelocity += dir.normalized * dashSpeed;

        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }

        StartCoroutine(DashTime(0.3f));
    }

    public void ResetDash()
    {
        hasDashed = false;

        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
            isDashing = false;
            wallJumped = false;

            rb.gravityScale = baseGravity;
            rb.linearDamping = 0;
        }
    }
}