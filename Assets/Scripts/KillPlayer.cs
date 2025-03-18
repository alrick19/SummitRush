using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public GameObject player;
    public Transform respawnPoint;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player.transform.position = respawnPoint.position;
        }
    }
}
