using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip magicSFX;
    [SerializeField] private AudioClip pourWaterSFX;
    [SerializeField] private AudioClip scoopWaterSFX;
    [SerializeField] private AudioClip doorCloseSFX;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    public float MusicVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupSourcesIfMissing();
        LoadVolume();
        ApplyVolume();
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    private void SetupSourcesIfMissing()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
    }

    private void LoadVolume()
    {
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }

    private void ApplyVolume()
    {
        if (musicSource != null)
            musicSource.volume = MusicVolume;

        if (sfxSource != null)
            sfxSource.volume = SFXVolume;
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        if (musicSource != null)
            musicSource.volume = MusicVolume;

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, MusicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        if (sfxSource != null)
            sfxSource.volume = SFXVolume;

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, SFXVolume);
        PlayerPrefs.Save();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null || musicSource == null) return;

        if (musicSource.clip == backgroundMusic && musicSource.isPlaying)
            return;

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(clip, SFXVolume);
    }

    public void PlayClick()
    {
        PlaySFX(clickSFX);
    }

    public void PlayMagic()
    {
        PlaySFX(magicSFX);
    }

    public void PlayPourWater()
    {
        PlaySFX(pourWaterSFX);
    }

    public void PlayScoopWater()
    {
        PlaySFX(scoopWaterSFX);
    }

    public void PlayDoorClose()
    {
        PlaySFX(doorCloseSFX);
    }
}