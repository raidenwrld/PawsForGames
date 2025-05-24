using UnityEngine;
using UnityEngine.UI;

public class MenuMusicControl : MonoBehaviour
{
    public Slider volumeSlider;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("PlayerVolume", 1f);

        SetVolume(savedVolume);

        volumeSlider.SetValueWithoutNotify(savedVolume);

        volumeSlider.onValueChanged.AddListener(SetVolume);

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            PlayerPrefs.SetFloat("PlayerVolume", volume);
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
