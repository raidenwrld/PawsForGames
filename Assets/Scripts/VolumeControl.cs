using UnityEngine;
using UnityEngine.UI;

public class SimpleVolumeControl : MonoBehaviour
{
    public Slider volumeSlider;
    private AudioSource playerAudio;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("PlayerVolume", 1f);
        volumeSlider.value = savedVolume;
        Invoke(nameof(FindPlayerAudio), 0.1f);

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void FindPlayerAudio()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerAudio = player.GetComponent<AudioSource>();

        SetVolume(volumeSlider.value);
    }

    void SetVolume(float volume)
    {
        if (playerAudio != null)
            playerAudio.volume = volume;

        PlayerPrefs.SetFloat("PlayerVolume", volume);
    }
}
