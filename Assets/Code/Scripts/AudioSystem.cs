using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance;

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

    [Space(20)]
    [Header("Sound Effect Clips")]
    public AudioClip buttonClick;
    
    private AudioSource currentMusicSource;
    private AudioSource nextMusicSource;
    private bool isCrossfading = false;

    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";


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
    public void PlayMusic(AudioClip clip, bool crossfade = true, float fadeDuration = 1f)
    {
        if (crossfade && currentMusicSource.isPlaying)
        {
            StartCoroutine(CrossfadeMusic(clip, fadeDuration));
        }
        else
        {
            currentMusicSource.clip = clip;
            currentMusicSource.Play();
        }
    }
    public void PlayMenuMusic(bool crossfade = true, float fadeDuration = 1f)
    {
        PlayMusic(menuMusic, crossfade, fadeDuration);
    }
    public void PlayGameplayMusic(bool crossfade = true, float fadeDuration = 1f)
    {
        PlayMusic(gameplayMusic, crossfade, fadeDuration);
    }
    private IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
    {
        if (isCrossfading) yield break;
        isCrossfading = true;

        // Setup next music source
        nextMusicSource.clip = newClip;
        nextMusicSource.volume = 0f;
        nextMusicSource.Play();

        float elapsed = 0f;
        float startVolume = currentMusicSource.volume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            currentMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);
            nextMusicSource.volume = Mathf.Lerp(0f, startVolume, t);

            yield return null;
        }

        currentMusicSource.Stop();
        currentMusicSource.volume = startVolume;

        // Swap sources
        AudioSource temp = currentMusicSource;
        currentMusicSource = nextMusicSource;
        nextMusicSource = temp;

        isCrossfading = false;
    }
    #endregion Music Functions


    #region Sound Effect Functions
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
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
