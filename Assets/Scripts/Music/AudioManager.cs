using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    Debug.Log("No AudioManager found in the scene!");
                }
            }
            return instance;
        }
    }

    [Header("音频源组件")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("脚步声")]
    public AudioClip footstepClip;  // 就一个音效

    [Header("UI滑块绑定")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
    {

    }

    void Start()
    {
        LoadSavedSettings();

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        UpdateAllVolumes();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateAllVolumes();
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateAllVolumes();
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        UpdateAllVolumes();
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    void UpdateAllVolumes()
    {
        if (musicSource != null)
            musicSource.volume = masterVolume * musicVolume;
        if (sfxSource != null)
            sfxSource.volume = masterVolume * sfxVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // 【脚步声】
    public void PlayFootstep()
    {
        if (sfxSource != null && footstepClip != null)
        {
            sfxSource.PlayOneShot(footstepClip);
        }
    }

    public void LoadSavedSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;

        UpdateAllVolumes();
    }
}