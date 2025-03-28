using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{
    private static bool isRespawning = false;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isRespawning)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.death);
            StartCoroutine(RespawnPlayer(other.gameObject));
        }
    }

    public IEnumerator RespawnPlayer(GameObject player)
    {
        if (isRespawning) yield break;

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

            ResetDoppelganger(LevelManager.Instance.GetRespawnPoint(), newPlayer.transform);
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
        Vector2 respawnPosition = LevelManager.Instance.GetRespawnPoint();
        GameObject playerPrefab = Resources.Load<GameObject>("Player");

        GameObject newPlayer = Instantiate(playerPrefab, respawnPosition, Quaternion.identity);
        LevelManager.Instance.SetPlayer(newPlayer);
        return newPlayer;
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