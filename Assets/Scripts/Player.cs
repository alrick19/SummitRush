using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f; // Max speed
    public float acceleration = 90f; 

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;

    [Header("Raycast Settings")]
    public float groundCheckDistance = 0.6f;
    public LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f; 
    }

    private void Update()
    {
        moveInput = InputManager.GetMoveInput();
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 newVelocity = rb.linearVelocity;

        if (Mathf.Abs(moveInput) > 0.01f) // Moving
        {
            newVelocity.x = Mathf.MoveTowards(newVelocity.x, moveInput * moveSpeed, acceleration * Time.fixedDeltaTime);
        }
        else // No input → STOP IMMEDIATELY
        {
            newVelocity.x = 0;
        }

        rb.linearVelocity = newVelocity;
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        // Debugging ground check raycast
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}