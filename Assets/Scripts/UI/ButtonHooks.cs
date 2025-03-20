using UnityEngine;
using UnityEngine.UI;

public class ButtonHooks : MonoBehaviour
{
    public Button level1Button;
    public Button mainMenuButton;
    public Button quitButton;

    private void Start()
    {
        if (level1Button) level1Button.onClick.AddListener(LoadLevel1);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(LoadMainMenu);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
    }

    private void LoadLevel1()
    {
        SceneHandler.Instance.LoadScene("Level1");
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
        if (level1Button) level1Button.onClick.RemoveListener(LoadLevel1);
        if (mainMenuButton) mainMenuButton.onClick.RemoveListener(LoadMainMenu);
        if (quitButton) quitButton.onClick.RemoveListener(QuitGame);
    }
}