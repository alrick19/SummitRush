using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour
{
    public GameObject playerPrefab; 
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
        isRespawning = true; // Block extra respawn triggers

        // Freeze player movement
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Disable player movement script
        Player movement = player.GetComponent<Player>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        yield return new WaitForSeconds(0.5f); // Wait before destroying

        Destroy(player); // Destroy the player

        yield return new WaitForSeconds(0.5f); // Wait before respawning

        // Spawn a new player at the respawn point
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

        
        ResetDoppelganger(respawnPoint.position);

        isRespawning = false; // Allow respawning again
    }

    private void ResetDoppelganger(Vector2 newPosition)
    {
        GameObject shadowDoppelganger = GameObject.FindGameObjectWithTag("doppelganger");

        if (shadowDoppelganger != null)
        {
            shadowDoppelganger.GetComponent<ShadowDoppelganger>().ResetDoppelganger(newPosition); 
        }
    }
}