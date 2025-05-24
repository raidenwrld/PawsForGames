using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        float savedVolume = PlayerPrefs.GetFloat("PlayerVolume", 1f);
        audioSource.volume = savedVolume;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            audioSource.Stop();
            audioSource.enabled = false;
        }
        else
        {
            audioSource.enabled = true;
            audioSource.volume = PlayerPrefs.GetFloat("PlayerVolume", 1f);
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }
}
