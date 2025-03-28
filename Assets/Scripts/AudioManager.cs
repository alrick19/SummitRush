using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99)]
public class AudioManager : SingletonMonoBehavior<AudioManager>
{
    [Header("--- Audio Source ---")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("--- Level Music Clips ---")]
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip level4Music;
    public AudioClip menuMusic; 

    private AudioClip currentLevelMusic;

    [Header("--- SFX Clips ---")]
    public AudioClip run;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip wallGrab;
    public AudioClip Climb;
    public AudioClip Dash;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayLevelMusic(scene.name);
    }

    private void PlayLevelMusic(string sceneName)
    {
        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "Level1":
                clipToPlay = level1Music;
                break;
            case "Level2":
                clipToPlay = level2Music;
                break;
            case "Level3":
                clipToPlay = level3Music;
                break;
            case "Level4":
                clipToPlay = level4Music;
                break;
            default:
                clipToPlay = menuMusic;
                break;
        }

        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            currentLevelMusic = clipToPlay; 
            musicSource.clip = clipToPlay;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayMenuMusic()
    {
        if (menuMusic != null && musicSource.clip != menuMusic)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void ResumeLevelMusic()
    {
        if (currentLevelMusic != null && musicSource.clip != currentLevelMusic)
        {
            musicSource.clip = currentLevelMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource != null && clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }
}