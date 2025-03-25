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
            if (doorToNextRoom) doorToNextRoom.SetActive(false); // unlock next room

            GameManager.Instance.RegisterRoomCompletion();
        }
    }

    public bool IsComplete() => roomComplete;
}