using UnityEngine;
using UnityEngine.UI;

public class PauseHandler : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button mainMenuButton;
    public Button quitButton;

    private void Start()
    {
        pauseMenuUI.SetActive(false);

        if (resumeButton) resumeButton.onClick.AddListener(Resume);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.IsPaused())
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        GameManager.Instance.PauseGame();
        pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
        GameManager.Instance.ResumeGame();
        pauseMenuUI.SetActive(false);
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.ResumeGame(); 
        SceneHandler.Instance.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        GameManager.Instance.ResumeGame();
        SceneHandler.Instance.QuitGame();
    }

    private void OnDestroy()
    {
        if (resumeButton) resumeButton.onClick.RemoveListener(Resume);
        if (mainMenuButton) mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        if (quitButton) quitButton.onClick.RemoveListener(QuitGame);
    }
}