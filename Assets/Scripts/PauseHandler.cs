using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    public GameObject pauseMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.IsPaused())
            {
                GameManager.Instance.ResumeGame();
                pauseMenu.SetActive(false);
            }
            else
            {
                GameManager.Instance.PauseGame();
                pauseMenu.SetActive(true);
            }
        }
    }
}