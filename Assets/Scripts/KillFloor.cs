using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillFloor : MonoBehaviour
{
    [SerializeField] private Transform startingRespawnPoint;
    [SerializeField] private int startingLives = 3;

    private Transform currentRespawnPoint;
    private int currentLives;

    private void Start()
    {
        currentRespawnPoint = startingRespawnPoint;
        currentLives = startingLives;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillFloor"))
        {
            currentLives--;

            if (currentLives > 0)
            {
                transform.position = currentRespawnPoint.position;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                GetComponentInChildren<MouseLook>().enabled = false;

                SceneManager.LoadScene("MainMenu");
            }
        }
        else if (other.CompareTag("Checkpoint"))
        {
            currentRespawnPoint = other.transform;
        }
    }
}
