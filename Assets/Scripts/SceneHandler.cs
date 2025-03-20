using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneHandler : SingletonMonoBehavior<SceneHandler>
{
    public Slider progressBar;//loading state

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
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        Debug.Log("Starting async loading of: " + _nextSceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_nextSceneName);
        asyncLoad.allowSceneActivation = false;

        float elapsedTime = 0f;
        float minLoadTime = 1.5f; // Simulated minimum loading time
        float displayProgress = 0f; // Stores smooth progress for UI

        // Try to find the progress bar in the Preload Scene
        if (progressBar == null)
        {
            GameObject sliderObj = GameObject.FindWithTag("ProgressBar");
            if (sliderObj) progressBar = sliderObj.GetComponent<Slider>();
        }

        while (!asyncLoad.isDone)
        {
            // Calculate the progress 
            float targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.8f);

            // Smoothly interpolate the display progress bar
            while (displayProgress < targetProgress)
            {
                displayProgress = Mathf.MoveTowards(displayProgress, targetProgress, Time.deltaTime);
                if (progressBar) progressBar.value = displayProgress;
                yield return null;
            }

            elapsedTime += Time.deltaTime;

            if (asyncLoad.progress >= 0.8f && elapsedTime >= minLoadTime)
            {
                // Smoothly move to 1 before switching scenes
                while (displayProgress < 1f)
                {
                    displayProgress = Mathf.MoveTowards(displayProgress, 1f, Time.deltaTime);
                    if (progressBar) progressBar.value = displayProgress;
                    yield return null;
                }

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

   
}