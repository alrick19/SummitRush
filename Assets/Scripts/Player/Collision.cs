using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Collision Variables")]
    public LayerMask groundLayer;
    public LayerMask hazardLayer;
    public float collisionRadius = 0.2f;
    public float wallCollisionRadius = 0.05f;

    [Space]
    public bool isGrounded;
    public bool isWalled;
    public bool rightWalled;
    public bool leftWalled;
    public bool onLedge;
    public Vector2 ledgePos;

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

        // positional wall
        rightWalled = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, wallCollisionRadius, groundLayer);
        leftWalled = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, wallCollisionRadius, groundLayer);

        // wall check
        isWalled = rightWalled || leftWalled;

        CheckForLedge(rightWalled, leftWalled);
    }

    /// <summary>
    /// Debugging method for visualizing Collision Overlap Circles
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // ground & wall overlap circles
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, wallCollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, wallCollisionRadius);

        // ledge overlap circles
        Gizmos.DrawWireSphere((Vector2)transform.position + (rightOffset + new Vector2(0, 0.3f)), wallCollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + (leftOffset + new Vector2(0, 0.3f)), wallCollisionRadius);


    }

    private void CheckForLedge(bool rightWall, bool leftWall)
    {
        Vector2 ledgeCheckOffset = new Vector2(0, 0.3f); // height above grab point
        Vector2 rightCheck = (Vector2)transform.position + rightOffset + ledgeCheckOffset;
        Vector2 leftCheck = (Vector2)transform.position + leftOffset + ledgeCheckOffset;

        bool noTileAboveRight = !Physics2D.OverlapCircle(rightCheck, wallCollisionRadius, groundLayer);
        bool noTileAboveLeft = !Physics2D.OverlapCircle(leftCheck, wallCollisionRadius, groundLayer);

        // prevent ledging if there are spikes 
        bool isHazardRight = Physics2D.OverlapCircle(rightCheck + new Vector2(0.5f, 0), wallCollisionRadius + 0.2f, hazardLayer);
        bool isHazardLeft = Physics2D.OverlapCircle(leftCheck + new Vector2(-0.5f, 0), wallCollisionRadius + 0.2f, hazardLayer);



        if (rightWall && noTileAboveRight && !isHazardRight)
        {
            onLedge = true;
            ledgePos = rightCheck;
        }
        else if (leftWall && noTileAboveLeft && !isHazardLeft)
        {
            onLedge = true;
            ledgePos = leftCheck;
        }
        else
        {
            onLedge = false;
        }
    }
}
