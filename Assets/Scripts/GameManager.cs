using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    public int totalCollectiblesInLevel;
    private int collectedCount;

    public int currentLevelIndex;
    public int unlockedLevelIndex; // highest unlocked level

    private bool isPaused = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        unlockedLevelIndex = PlayerPrefs.GetInt("UnlockedLevelIndex", 0);
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void RegisterCollectible()
    {
        collectedCount++;
        if (collectedCount >= totalCollectiblesInLevel)
        {
            Debug.Log("All collectibles gathered! Ready to finish level.");
        }
    }

    public bool CanFinishLevel()
    {
        return collectedCount >= totalCollectiblesInLevel;
    }

    public void CompleteLevel()
    {
        if (currentLevelIndex >= unlockedLevelIndex)
        {
            unlockedLevelIndex = currentLevelIndex + 1;
            PlayerPrefs.SetInt("UnlockedLevelIndex", unlockedLevelIndex);
            PlayerPrefs.Save();
        }

        SceneHandler.Instance.LoadScene(SceneUtility.GetScenePathByBuildIndex(currentLevelIndex + 1));
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        // Show Pause Menu UI
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        // Hide Pause Menu UI
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void ResetLevelProgress()
    {
        collectedCount = 0;
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("UnlockedLevelIndex", 0);
        PlayerPrefs.Save();
        SceneHandler.Instance.LoadScene("Level1");
    }

    public void ContinueGame()
    {
        int levelToLoad = Mathf.Clamp(unlockedLevelIndex, 0, SceneManager.sceneCountInBuildSettings - 1);
        SceneHandler.Instance.LoadScene(SceneUtility.GetScenePathByBuildIndex(levelToLoad));
    }
}