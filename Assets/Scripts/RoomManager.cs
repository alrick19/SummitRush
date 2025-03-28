using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    public int totalCollectibles;
    private int collectedCount;
    public GameObject doorToNextRoom;

    private bool roomComplete = false;

    private void Start()
    {
        totalCollectibles = GameObject.FindGameObjectsWithTag("Collectible")
            .Count(obj => obj.transform.IsChildOf(transform)); // count only within this room
    }

    public void RegisterCollectible()
    {
        collectedCount++;

        if (collectedCount >= totalCollectibles && !roomComplete)
        {
            roomComplete = true;
            if (doorToNextRoom)
            {
                doorToNextRoom.SetActive(false); // unlock next room
                AudioManager.Instance.PlaySFX(AudioManager.Instance.complete);
            }

            GameManager.Instance.RegisterRoomCompletion();
        }
    }

    public bool IsComplete() => roomComplete;

    public void ResetCollectibles()
    {
        if (roomComplete) return;

        collectedCount = 0;

        var collectibles = GetComponentsInChildren<Collectible>(includeInactive: true);
        foreach (var collectible in collectibles)
        {
            collectible.ResetCollectible();
        }
    }
}