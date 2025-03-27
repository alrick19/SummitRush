using UnityEngine;
using System.Collections;

public class ShadowDoppelganger : MonoBehaviour
{
    private Collider2D col;
    private bool waitingForPlayerMove = true; // Flag to track if we're waiting for movement so there isnt a kill loop on respawn
    private Transform playerTransform;
    private Vector2 lastPlayerPosition;

    private ShadowAnimationScript shadowAnim;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        shadowAnim = GetComponentInChildren<ShadowAnimationScript>();
    }

    private void Update()
    {
        if (PlayerMovementTracker.Instance != null)
        {
            var snapshot = PlayerMovementTracker.Instance.GetDelayedSnapshot();
            transform.position = snapshot.position;

            // Apply animation state
            if (shadowAnim != null)
            {
                shadowAnim.SetStates(
                    snapshot.isGrounded,
                    snapshot.isSliding,
                    snapshot.isJumping,
                    snapshot.isDashing
                );
            }
        }

        // Player movement detection after respawn
        if (waitingForPlayerMove && playerTransform != null)
        {
            if (Vector2.Distance(playerTransform.position, lastPlayerPosition) > 0.1f)
            {
                waitingForPlayerMove = false;
                StartCoroutine(EnableCollisionWithDelay());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && col.enabled)
        {
            KillPlayer killPlayer = FindAnyObjectByType<KillPlayer>();
            if (killPlayer != null)
            {
                StartCoroutine(killPlayer.RespawnPlayer(other.gameObject));
            }
        }
    }

    public void ResetDoppelganger(Vector2 respawnPosition, Transform newPlayerTransform)
    {
        Vector2 spawnOffset = new Vector2(2f, 0); 
        transform.position = respawnPosition + spawnOffset;

        col.enabled = false;

        playerTransform = newPlayerTransform;
        lastPlayerPosition = playerTransform.position;
        waitingForPlayerMove = true;

        PlayerMovementTracker.Instance.ResetMovementHistory();
    }

    private IEnumerator EnableCollisionWithDelay()
    {
        yield return new WaitForSeconds(0.2f); // Wait x secs after player moves
        col.enabled = true; // Re-enable collision
    }
}