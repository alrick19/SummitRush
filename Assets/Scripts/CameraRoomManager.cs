using UnityEngine;
using Unity.Cinemachine;

public class CameraRoomManager : MonoBehaviour
{
    public CinemachineCamera virtualCam;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (virtualCam != null && player != null)
        {
            virtualCam.gameObject.SetActive(false);
            virtualCam.Follow = player.transform;
            virtualCam.LookAt = player.transform;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.gameObject.SetActive(true);
            virtualCam.Follow = other.transform;
            virtualCam.LookAt = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.gameObject.SetActive(false);
        }
    }
}