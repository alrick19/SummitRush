using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private Transform defaultRespawnPoint;
    private Vector2 currentRespawnPoint;
    private GameObject player;
    private CameraRoomManager currentRoom;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        player = GameObject.FindGameObjectWithTag("Player");

        if (defaultRespawnPoint != null)
        {
            currentRespawnPoint = defaultRespawnPoint.position;
        }
        else if (player != null)
        {
            currentRespawnPoint = player.transform.position;
        }

    }

    public void SetRespawnPoint(Vector2 newPoint)
    {
        currentRespawnPoint = newPoint;
    }

    public Vector2 GetRespawnPoint()
    {
        return currentRespawnPoint;
    }

    public GameObject GetPlayer()
    {
        return player != null ? player : GameObject.FindGameObjectWithTag("Player");
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }

    public void SetCurrentRoom(CameraRoomManager room)
    {
        currentRoom = room;
    }

    public CameraRoomManager GetCurrentRoom()
    {
        return currentRoom;
    }
}