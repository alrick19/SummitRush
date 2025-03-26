using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class SceneHandler : SingletonMonoBehavior<SceneHandler>
{
    public TextMeshProUGUI loadingText;
    private string _nextSceneName = "MainMenu";//default to main menu

    private void Start()
    {
        // If we're in the Preload Scene, start loading the Main Menu
        if (SceneManager.GetActiveScene().name == "Preload")
        {
            StartCoroutine(LoadSceneAsync());
        }
    }

    public void LoadScene(string sceneName)
    {
        _nextSceneName = sceneName;
        StartCoroutine(LoadPreloadScene());
    }

    public void ReloadScene()
    {
        _nextSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadPreloadScene());
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private IEnumerator LoadPreloadScene()
    {
        SceneManager.LoadScene("Preload"); // Load the Preload Scene
        yield return new WaitForSeconds(0.5f); 
        if (loadingText == null)
        {
            var foundText = GameObject.FindWithTag("LoadingText");
            if (foundText) loadingText = foundText.GetComponent<TextMeshProUGUI>();
        }

        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        if (loadingText)
            loadingText.text = $"Loading {GetFormattedSceneName(_nextSceneName)}...";

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_nextSceneName);
        asyncLoad.allowSceneActivation = false;

        float elapsedTime = 0f;
        float minLoadTime = 1.5f;

        //wait a minimum time to show spinner for a moment
        while (elapsedTime < minLoadTime || asyncLoad.progress < 0.9f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Activate the scene after delay
        asyncLoad.allowSceneActivation = true;
    }

    private string GetFormattedSceneName(string rawName)
    {
        if (rawName.StartsWith("Level"))
        {
            // Convert "Level#" to "Level #"
            string numberPart = rawName.Substring("Level".Length);
            return $"Level {numberPart}";
        }
        else if (rawName == "MainMenu")
        {
            return "Main Menu";
        }

        return rawName;
    }

   
}