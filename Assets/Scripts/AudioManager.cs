using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99)]
public class AudioManager : SingletonMonoBehavior<AudioManager>
{
    [Header("--- Audio Source ---")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource loopingSFXSource;

    [Header("--- Level Music Clips ---")]
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip level4Music;
    public AudioClip menuMusic; 

    private AudioClip currentLevelMusic;

    [Header("--- SFX Clips ---")]
    public AudioClip walkLoop;//
    public AudioClip jump;//
    public AudioClip land;
    public AudioClip wallGrab //
    public AudioClip wallSlideLoop;//
    public AudioClip wallClimbLoop;//
    public AudioClip dash;//
    public AudioClip death;//

    private AudioClip currentLoopingClip;


    protected override void Awake()
    {
        base.Awake();
        if (loopingSFXSource == null)
        {
            loopingSFXSource = gameObject.AddComponent<AudioSource>();
            loopingSFXSource.loop = true;
            loopingSFXSource.playOnAwake = false;
        }
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

    public void PlayLoopingSFX(AudioClip clip)
    {
        if (loopingSFXSource == null || clip == null) return;

        if (loopingSFXSource.clip != clip)
        {
            loopingSFXSource.clip = clip;
            loopingSFXSource.Play();
            currentLoopingClip = clip;
        }
    }

    public void StopLoopingSFX(AudioClip clip = null)
    {
        if (loopingSFXSource != null && (clip == null || loopingSFXSource.clip == clip))
        {
            loopingSFXSource.Stop();
            loopingSFXSource.clip = null;
            currentLoopingClip = null;
        }
    }
}