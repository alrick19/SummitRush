using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour
{
    public GameObject playerPrefab; // reference player prefab for respawning
    public Transform respawnPoint;
    
    private static bool isRespawning = false; // Prevent multiple respawns

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isRespawning)
        {
            StartCoroutine(RespawnPlayer(other.gameObject));
        }
    }

    private IEnumerator RespawnPlayer(GameObject player)
    {
        isRespawning = true; // block extra respawn triggers

        // Freeze player movement
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.bodyType = RigidbodyType2D.Static; 
        }

        // Disable movement 
        Player movement = player.GetComponent<Player>(); 
        if (movement != null)
        {
            movement.enabled = false; 
        }

        yield return new WaitForSeconds(0.5f); // Wait before destroying

        Destroy(player); // Destroy the player

        yield return new WaitForSeconds(0.5f); // Wait before respawning

        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        

        // Re-enable movement for the new player
        Rigidbody2D newRb = newPlayer.GetComponent<Rigidbody2D>();
        if (newRb != null)
        {
            newRb.bodyType = RigidbodyType2D.Dynamic; 
        }

        Player newMovement = newPlayer.GetComponent<Player>();
        if (newMovement != null)
        {
            newMovement.enabled = true; 
        }

        isRespawning = false; // Allow respawning again
    }
}