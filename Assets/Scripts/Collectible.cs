using UnityEngine;

public class Collectible : MonoBehaviour
{
    private RoomManager roomManager;

    private void Start()
    {
        roomManager = GetComponentInParent<RoomManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.item);
            roomManager?.RegisterCollectible();
            gameObject.SetActive(false);
        }
    }

    public void ResetCollectible()
    {
        gameObject.SetActive(true);
    }
}