using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    public int totalCollectiblesInLevel;
    private int collectedCount;

    public int currentLevelIndex;
    public int unlockedLevelIndex;

    private bool isPaused = false;

    protected override void Awake()
    {
        base.Awake(); // Ensures singleton logic from base class

        // Only run on initial load
        unlockedLevelIndex = PlayerPrefs.GetInt("UnlockedLevelIndex", 0);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Level"))
        {
            InitializeLevel();
        }
    }

    public void InitializeLevel()
    {
        ResetLevelProgress();
        totalCollectiblesInLevel = GameObject.FindGameObjectsWithTag("Collectible").Length;
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
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

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
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    public bool IsPaused() => isPaused;

    public void ResetLevelProgress()
    {
        collectedCount = 0;
        totalCollectiblesInLevel = 0;
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