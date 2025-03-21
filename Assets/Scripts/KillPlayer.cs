using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour
{
    private static Vector2 respawnPoint;
    private static bool isRespawning = false;

    private void Start()
    {
        if (respawnPoint == Vector2.zero) // Set initial respawn point to the player's start position
        {
            GameObject player = FindPlayer();
            if (player != null)
            {
                respawnPoint = player.transform.position;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isRespawning)
        {
            StartCoroutine(RespawnPlayer(other.gameObject));
        }
    }

    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }

    public IEnumerator RespawnPlayer(GameObject player)
    {
        isRespawning = true;

        Destroy(player); // Destroy the player

        yield return new WaitForSeconds(0.5f); // Wait before respawning

        // Find or instantiate the player at the respawn point
        GameObject newPlayer = SpawnNewPlayer();

        // Update movement tracker with the new player
        PlayerMovementTracker.Instance.SetNewPlayer(newPlayer.transform);

        // Move Doppelgä=anger and reset tracking
        ResetDoppelganger(respawnPoint, newPlayer.transform);

        isRespawning = false;
    }
    

    private GameObject SpawnNewPlayer()
    {
        // check for existing player first 
        GameObject existingPlayer = FindPlayer();
        if (existingPlayer != null)
        {
            existingPlayer.transform.position = respawnPoint;
            return existingPlayer;
        }

        // Otherwise, instantiate a new player
        GameObject newPlayer = Instantiate(Resources.Load<GameObject>("Player"), respawnPoint, Quaternion.identity);
        return newPlayer;
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    private void ResetDoppelganger(Vector2 newPosition, Transform newPlayerTransform)
    {
        GameObject shadowDoppelganger = GameObject.FindGameObjectWithTag("doppelganger");
        if (shadowDoppelganger != null)
        {
            shadowDoppelganger.GetComponent<ShadowDoppelganger>().ResetDoppelganger(newPosition, newPlayerTransform);
        }
    }
}