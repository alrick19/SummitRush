using UnityEngine;
using System.Collections;

public class ShadowDoppelganger : MonoBehaviour
{
    private Collider2D col;
    private bool waitingForPlayerMove = true; // Flag to track if we're waiting for movement so there isnt a kill loop on respawn
    private Transform playerTransform;
    private Vector2 lastPlayerPosition;
    private bool isReactivating = false;
    private float initialSpawnTime;

    private ShadowAnimationScript shadowAnim;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovementTracker.Instance.SetNewPlayer(player.transform);
            ResetDoppelganger(player.transform.position, player.transform);
        }
    }

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        shadowAnim = GetComponentInChildren<ShadowAnimationScript>();
    }

    private void Update()
    {
        
        if (!isReactivating && !waitingForPlayerMove && PlayerMovementTracker.Instance != null)
        {
            if (PlayerMovementTracker.Instance.HasSnapshots())
            {
                var snapshot = PlayerMovementTracker.Instance.GetDelayedSnapshot();
                transform.position = snapshot.position;
        
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
        }

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
        transform.position = respawnPosition; // same position as player

        col.enabled = false;
        playerTransform = newPlayerTransform;
        lastPlayerPosition = playerTransform.position;
        waitingForPlayerMove = true;
        isReactivating = true;

        initialSpawnTime = Time.time;

        PlayerMovementTracker.Instance.ResetMovementHistory();

        StartCoroutine(AllowTrackingAfterDelay(0.5f));
    }

    private IEnumerator AllowTrackingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isReactivating = false;
    }

    private IEnumerator EnableCollisionWithDelay()
    {
        yield return new WaitForSeconds(1f); // Wait 1 sec after movement
        col.enabled = true;
    }
}