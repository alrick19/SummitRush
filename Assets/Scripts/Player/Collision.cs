using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Collision Variables")]
    public LayerMask groundLayer;
    public float collisionRadius = 0.2f;

    [Space]
    public bool isGrounded;
    public bool isWalled;
    public bool rightWalled;
    public bool leftWalled;

    private Vector2 bottomOffset;
    private Vector2 leftOffset;
    private Vector2 rightOffset;

    private CapsuleCollider2D playerCollider;

    void Awake()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();
        bottomOffset = new Vector2(0, -playerCollider.bounds.extents.y);

        float width = playerCollider.bounds.extents.x; // Half width of the collider
        float height = playerCollider.bounds.extents.y * 0.5f; // Middle of the collider

        leftOffset = new Vector2(-width - 0.05f, height * 0.5f);
        rightOffset = new Vector2(width + 0.05f, height * 0.5f);
    }

    void Update()
    {
        // ground check
        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

        // wall check
        isWalled = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        // positional wall
        rightWalled = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        leftWalled = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

    }

    /// <summary>
    /// Debugging method for visualizing Collision Overlap Circles
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }
}
