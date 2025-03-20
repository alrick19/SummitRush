using UnityEngine;

public class ShadowDoppelganger : MonoBehaviour
{
    private void Start()
    {
        // Ensure it starts mimicking the player
        if (PlayerMovementTracker.Instance == null)
        {
            Debug.LogError("ShadowDoppelganger: No PlayerMovementTracker found!");
        }
    }

    private void Update()
    {
        if (PlayerMovementTracker.Instance != null)
        {
            // Get the delayed position from PlayerMovementTracker
            transform.position = PlayerMovementTracker.Instance.GetDelayedPosition();
        }
    }

    // Called by KillPlayer to reset Doppelganger when the player respawns
    public void ResetDoppelganger(Vector2 respawnPosition)
    {
        transform.position = respawnPosition; 
        PlayerMovementTracker.Instance.ResetMovementHistory(); 
    }
}