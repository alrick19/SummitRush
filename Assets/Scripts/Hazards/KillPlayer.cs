using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;


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
        if (isRespawning)
            yield break;

        isRespawning = true;

        Destroy(player);

        ResetRoom();
        yield return new WaitForSeconds(0.5f);

        GameObject newPlayer = SpawnNewPlayer();

        UpdateAllCamerasFollowTarget(newPlayer.transform);

        if (SceneManager.GetActiveScene().name == "Level4")
        {
            if (PlayerMovementTracker.Instance != null)
            {
                PlayerMovementTracker.Instance.SetNewPlayer(newPlayer.transform);
            }

            ResetDoppelganger(respawnPoint, newPlayer.transform);
        }

        isRespawning = false;
    }

    private void ResetRoom()
    {
        CameraRoomManager cameraRoomManager = GetComponentInParent<CameraRoomManager>();
        if (cameraRoomManager != null)
        {
            cameraRoomManager.ResetCollectibles();
            cameraRoomManager.ResetHazards();
        }
        
    }

    private GameObject SpawnNewPlayer()
    {
        GameObject existingPlayer = FindPlayer();

        if (existingPlayer != null && !existingPlayer.Equals(null))
        {
            existingPlayer.transform.position = respawnPoint;
            return existingPlayer;
        }

        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint, Quaternion.identity);
  
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

    private void UpdateAllCamerasFollowTarget(Transform newTarget)
    {
        CinemachineCamera[] allCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);

        foreach (var cam in allCams)
        {
            cam.Follow = newTarget;
            cam.LookAt = newTarget;
        }
    }
}