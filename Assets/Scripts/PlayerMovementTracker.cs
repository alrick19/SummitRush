using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTracker : SingletonMonoBehavior<PlayerMovementTracker>
{
    private Transform playerPrefab; 
    [SerializeField] private float delayTime = 3f; // Time delay in seconds

    private Queue<(Vector2 position, float timeStamp)> movementHistory = new Queue<(Vector2, float)>();

    void Update()
    {
        // ensure we are tracking the player
        if  (playerPrefab == null)
        {
            FindPlayer();
        }

        if  (playerPrefab != null)
        {
            // Store the player's position along with the current time
            movementHistory.Enqueue((playerPrefab.position, Time.time));
        }

        // reemove positions older than the delay time
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
        else
        {
            Debug.LogWarning("PlayerMovementTracker: No Player found in the scene.");
        }
    }

    // Call this when a new player spawns to update tracking
    public void SetNewPlayer(Transform newPlayer)
    {
        playerPrefab = newPlayer;
        ResetMovementHistory(); // Clear old data to sync with new player
    }
}