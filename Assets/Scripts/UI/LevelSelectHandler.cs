using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectHandler : MonoBehaviour
{
    public int levelIndex; //level1 : 1, Level2 : 2 ...
    public Button levelButton;
    public Image lockImage;
    public string levelSceneName; 

    private void Start()
    {
        int unlockedIndex = PlayerPrefs.GetInt("UnlockedLevelIndex", 2); // Default unlocks Level 1

        bool isUnlocked = levelIndex <= unlockedIndex - 1;

        levelButton.interactable = isUnlocked;
        lockImage.gameObject.SetActive(!isUnlocked);

        if (isUnlocked)
        {
            levelButton.onClick.AddListener(() =>
            {
                SceneHandler.Instance.LoadScene(levelSceneName);
            });
        }
    }
}