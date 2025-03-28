using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99)]
public class AudioManager : SingletonMonoBehavior<AudioManager>
{
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float loopSFXVolume = 1f;

    [Header("--- Audio Source ---")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource loopingSFXSource;
    [SerializeField] private AudioSource ambientSource; //  like wind

    [Header("--- Level Music Clips ---")]
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip level4Music;
    public AudioClip menuMusic; 

    private AudioClip currentLevelMusic;

    [Header("--- Ambient Layer Clips ---")]
    public AudioClip level1Ambient;
    public AudioClip level2Ambient;
    public AudioClip level3Ambient;
    public AudioClip level4Ambient;
    public AudioClip menuAmbient;

    [Header("--- SFX Clips ---")]
    public AudioClip walkLoop;//
    public AudioClip jump;//
    public AudioClip land;
    public AudioClip wallJump;//
    public AudioClip wallSlideLoop;//
    public AudioClip wallClimbLoop;//
    public AudioClip dash;//
    public AudioClip death;//
    public AudioClip item;//
    public AudioClip complete;//
    public AudioClip icicle;//
    public AudioClip fallBlock;//
    public AudioClip trampoline;//
    


    protected override void Awake()
    {
        base.Awake();

        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.loop = true;
            ambientSource.playOnAwake = false;
        }
        if (loopingSFXSource == null)
        {
            loopingSFXSource = gameObject.AddComponent<AudioSource>();
            loopingSFXSource.loop = true;
            loopingSFXSource.playOnAwake = false;
        }

        musicSource.volume = musicVolume;
        SFXSource.volume = sfxVolume;
        loopingSFXSource.volume = loopSFXVolume;
        ambientSource.volume = ambientVolume;
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
        AudioClip ambientClip = null;

        switch (sceneName)
        {
            case "Level1":
                clipToPlay = level1Music;
                ambientClip = level1Ambient;
                break;
            case "Level2":
                clipToPlay = level2Music;
                ambientClip = level2Ambient;
                break;
            case "Level3":
                clipToPlay = level3Music;
                ambientClip = level3Ambient;
                break;
            case "Level4":
                clipToPlay = level4Music;
                ambientClip = level4Ambient;
                break;
            default:
                clipToPlay = menuMusic;
                ambientClip = menuAmbient;
                break;
        }

        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            currentLevelMusic = clipToPlay; 
            musicSource.clip = clipToPlay;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (ambientClip != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.Play();
        }
        else
        {
            ambientSource.Stop();
            ambientSource.clip = null;
        }
    }

    public void PauseLevelMusic()
    {
        musicSource.Pause();
    }

    public void ResumeLevelMusic()
    {
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource != null && clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    public void PlayLoop(AudioClip clip)
    {
        if (clip == null || loopingSFXSource.clip == clip) return;

        loopingSFXSource.clip = clip;
        loopingSFXSource.loop = true;
        loopingSFXSource.Play();
    }

    public void StopLoop(AudioClip clip = null)
    {
        if (loopingSFXSource == null) return;

        if (clip == null || loopingSFXSource.clip == clip)
        {
            loopingSFXSource.Stop();
            loopingSFXSource.clip = null;
        }
    }
}