using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTracker : SingletonMonoBehavior<PlayerMovementTracker>
{
    private Transform playerPrefab; 
    [SerializeField] private float delayTime = 1.5f; // Time delay in seconds

    private Queue<PlayerStateSnapshot> movementHistory = new Queue<PlayerStateSnapshot>();

    private struct PlayerStateSnapshot
    {
        public Vector2 position;
        public float timeStamp;

        public bool isJumping;
        public bool isDashing;
        public bool isGrounded;
        public bool isSliding;
    }

    void Update()
    {
        if (playerPrefab == null)
            FindPlayer();

        if (playerPrefab != null)
        {
            Player player = playerPrefab.GetComponent<Player>();
            Collision collision = playerPrefab.GetComponent<Collision>();

            PlayerStateSnapshot snapshot = new PlayerStateSnapshot
            {
                position = playerPrefab.position,
                timeStamp = Time.time,
                isJumping = player.isJumping,
                isDashing = player.isDashing,
                isGrounded = collision.isGrounded,
                isSliding = player.isSliding
            };

            movementHistory.Enqueue(snapshot);
        }

        while (movementHistory.Count > 0 && Time.time - movementHistory.Peek().timeStamp > delayTime)
        {
            movementHistory.Dequeue();
        }
    }

    public Vector2 GetDelayedPosition()
    {
        // If we have a valid delayed position, return it
        if (movementHistory.Count > 0)
        {
            return movementHistory.Peek().position;
        }

        // If no history exists yet, just return the current player position
        return playerPrefab != null ? playerPrefab.position : Vector2.zero;
    }

    public void ResetMovementHistory()
    {
        movementHistory.Clear();
    }

    // Finds the latest player instance in the scene
    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPrefab = player.transform;
        }
    }

    // Call this when a new player spawns to update tracking
    public void SetNewPlayer(Transform newPlayer)
    {
        playerPrefab = newPlayer;
        ResetMovementHistory(); // Clear old data to sync with new player
    }

    public (Vector2 position, bool isJumping, bool isDashing, bool isGrounded, bool isSliding) GetDelayedSnapshot()
    {
        if (movementHistory.Count > 0)
        {
            var snapshot = movementHistory.Peek();
            return (snapshot.position, snapshot.isJumping, snapshot.isDashing, snapshot.isGrounded, snapshot.isSliding);
        }
    
        return (playerPrefab.position, false, false, true, false);
    }
}