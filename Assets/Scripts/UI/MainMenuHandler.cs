using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public Button playButton;
    public Button mainMenuButton;
    public Button quitButton;

    private void Start()
    {
        if (playButton) playButton.onClick.AddListener(PlayGame);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(LoadMainMenu);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
    }

    public void PlayGame()
    {
        if (PlayerPrefs.HasKey("UnlockedLevelIndex"))
            GameManager.Instance.ContinueGame();
        else
            GameManager.Instance.StartNewGame();
    }

    private void LoadMainMenu()
    {
        SceneHandler.Instance.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
        SceneHandler.Instance.QuitGame();
    }

    private void OnDestroy()
    {
        if (playButton) playButton.onClick.RemoveListener(PlayGame);
        if (mainMenuButton) mainMenuButton.onClick.RemoveListener(LoadMainMenu);
        if (quitButton) quitButton.onClick.RemoveListener(QuitGame);
    }
}