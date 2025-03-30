using UnityEngine;

public class SettingsMenuToggle : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
    }
}