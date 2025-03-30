using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider ambientSlider;

    private void Start()
    {
        var audio = AudioManager.Instance;

        musicSlider.value = audio.musicVolume;
        sfxSlider.value = audio.sfxVolume;
        ambientSlider.value = audio.ambientVolume;

        musicSlider.onValueChanged.AddListener(audio.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(audio.SetSFXVolume);
        ambientSlider.onValueChanged.AddListener(audio.SetAmbientVolume);
    }

    private void OnDestroy()
    {
        var audio = AudioManager.Instance;
        musicSlider.onValueChanged.RemoveListener(audio.SetMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(audio.SetSFXVolume);
        ambientSlider.onValueChanged.RemoveListener(audio.SetAmbientVolume);
    }
}