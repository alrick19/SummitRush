using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private Transform newRespawnPoint; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer killPlayer = FindAnyObjectByType<KillPlayer>();
            if (killPlayer != null)
            {
                killPlayer.SetRespawnPoint(newRespawnPoint.position);
            }
        }
    }
}