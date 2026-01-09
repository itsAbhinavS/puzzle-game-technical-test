using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Space(20)]
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Space(20)]
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    [Space(20)]
    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;

    [Space(20)]
    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip winMusic;

    [Space(20)]
    [Header("Sound Effect Clips")]
    public AudioClip buttonClick;
    public AudioClip deniedSound;
    public AudioClip collectSound;
  
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";
    private bool isCrossfading = false;


    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion

        InitializeAudioSources();
        LoadVolumeSettings();
    }



    private void InitializeAudioSources()
    {
        // Configure Music Sources
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // Configure SFX Source
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }



    #region Music Functions
    public void PlayMenuMusic(float fadeDuration = 1f)
    {
        CrossfadeMusic(menuMusic, fadeDuration);
    }
    public void PlayGameplayMusic(float fadeDuration = 1f)
    {
        CrossfadeMusic(gameplayMusic, fadeDuration);
    }
    public void PlayWinMusic(float fadeDuration = 1f)
    {
        CrossfadeMusic(winMusic, fadeDuration);
    }
    private void CrossfadeMusic(AudioClip musicClip, float duration)
    {
        if (musicSource.resource == musicClip) return;

        if (isCrossfading) DOTween.Kill(this);
        
        isCrossfading = true;

        float volume = GetMusicVolume();

        if (musicSource.resource == null)
        {
            musicSource.volume = 0;
            musicSource.resource = musicClip;
            musicSource.Play();
            musicSource.DOFade(1, duration);

            isCrossfading = false;
        }
        else 
        {
            musicSource.DOFade(0, duration).OnComplete(() =>
            {
                musicSource.volume = 0;
                musicSource.resource = musicClip;
                musicSource.Play();
                musicSource.DOFade(1, duration);

                isCrossfading = false;
            });
        }
    }
    #endregion Music Functions



    #region Sound Effect Functions
    private void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
    }
    public void PlayDeniedSound()
    {
        PlaySFX(deniedSound);
    }
    public void PlayAuraCollectSound()
    {
        PlaySFX(collectSound);
    }
    #endregion Sound Effect Functions



    #region Volume Control (with Audio Mixer)
    public void SetMusicVolume(float volume)
    {
        float db = LinearToDecibel(volume);
        audioMixer.SetFloat(MUSIC_VOLUME, db);
        PlayerPrefs.SetFloat(MUSIC_VOLUME, volume);
    }

    public void SetSFXVolume(float volume)
    {
        float db = LinearToDecibel(volume);
        audioMixer.SetFloat(SFX_VOLUME, db);
        PlayerPrefs.SetFloat(SFX_VOLUME, volume);
    }
    
    private float LinearToDecibel(float linear)
    {
        // Convert linear volume (0-1) to decibel (-80 to 0)
        if (linear <= 0f) return -80f;
        return Mathf.Log10(linear) * 20f;
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME, 1f);
    }
    #endregion Volume Control (with Audio Mixer)



    private void LoadVolumeSettings()
    {
        SetMusicVolume(GetMusicVolume());
        SetSFXVolume(GetSFXVolume());
    }
}
