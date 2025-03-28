using System.Collections.Generic;
using UnityEngine;

public struct MovementSnapshot
{
    public Vector2 position;
    public bool isJumping;
    public bool isDashing;
    public bool isGrounded;
    public bool isSliding;

    public MovementSnapshot(Vector2 position, bool isJumping, bool isDashing, bool isGrounded, bool isSliding)
    {
        this.position = position;
        this.isJumping = isJumping;
        this.isDashing = isDashing;
        this.isGrounded = isGrounded;
        this.isSliding = isSliding;
    }
}

public class PlayerMovementTracker : SingletonMonoBehavior<PlayerMovementTracker>
{
    private Transform playerPrefab; 
    [SerializeField] private float delayTime = 1.5f;

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
        if (playerPrefab == null || playerPrefab.Equals(null))
        {
            playerPrefab = null;
            return;
        }

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

    public MovementSnapshot GetDelayedSnapshot()
    {
        if (playerPrefab == null || playerPrefab.Equals(null))
            return new MovementSnapshot();

        if (movementHistory.Count > 0)
        {
            var s = movementHistory.Peek();
            return new MovementSnapshot(s.position, s.isJumping, s.isDashing, s.isGrounded, s.isSliding);
        }

        return new MovementSnapshot(playerPrefab.position, false, false, true, false);
    }

    public void ResetMovementHistory()
    {
        movementHistory.Clear();
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPrefab = player.transform;
        }
    }

    public void SetNewPlayer(Transform newPlayer)
    {
        playerPrefab = newPlayer;
        ResetMovementHistory();
    }

    public bool HasSnapshots()
    {
        return movementHistory.Count > 0;
    }
}