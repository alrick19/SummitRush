using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    private int totalRoomsInLevel;
    private int completedRooms;

    public int currentLevelIndex;
    public int unlockedLevelIndex;

    private bool isPaused = false;

    protected override void Awake()
    {
        base.Awake();  
        unlockedLevelIndex = PlayerPrefs.GetInt("UnlockedLevelIndex", 2);//level 1 defualt.
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
        
        RoomManager[] rooms = FindObjectsByType<RoomManager>(FindObjectsSortMode.None);
        totalRoomsInLevel = rooms.Length;
        completedRooms = 0;
        InputManager.UnlockInput();
    }

    public void RegisterRoomCompletion()
    {
        completedRooms++;
    }

    public bool CanFinishLevel()
    {
        return completedRooms >= totalRoomsInLevel;
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

        string nextSceneName = GetSceneNameFromBuildIndex(currentLevelIndex + 1);
        SceneHandler.Instance.LoadScene(nextSceneName);
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
        totalRoomsInLevel = 0;
        completedRooms = 0;
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("UnlockedLevelIndex", 2);
        PlayerPrefs.Save();
        SceneHandler.Instance.LoadScene("Level1");
    }

    public void ContinueGame()
    {
        int levelToLoad = Mathf.Clamp(unlockedLevelIndex, 0, SceneManager.sceneCountInBuildSettings - 1);
        SceneHandler.Instance.LoadScene(GetSceneNameFromBuildIndex(levelToLoad));    
    }

    private string GetSceneNameFromBuildIndex(int buildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex); // e.g. "Assets/Scenes/Level1.unity"
        string fileName = System.IO.Path.GetFileNameWithoutExtension(path); // "Level1"
        return fileName;
    }
}