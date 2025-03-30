using UnityEngine;
using Unity.Cinemachine;

public class CameraRoomManager : MonoBehaviour
{
    public CinemachineCamera virtualCam;
    private RoomHazardManager hazardManager; // manages resettable hazards in this room


    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (virtualCam != null && player != null)
        {
            virtualCam.gameObject.SetActive(false);
            virtualCam.Follow = player.transform;
            virtualCam.LookAt = player.transform;
        }

        // hazardManager component should be attached to the room
        hazardManager = GetComponent<RoomHazardManager>();

    }

    public void ResetHazards()
    {
        if (hazardManager)
            hazardManager.ResetAllHazards();
    }

    public void ResetCollectibles()
    {
        RoomManager rm = GetComponentInChildren<RoomManager>();
        if (rm)
            rm.ResetCollectibles();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        { 
            LevelManager.Instance.SetCurrentRoom(this);
            virtualCam.gameObject.SetActive(true);
            virtualCam.Follow = other.transform;
            virtualCam.LookAt = other.transform;
        }

        ResetHazards();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.gameObject.SetActive(false);
        }
    }
}