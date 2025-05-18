using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillFloor : MonoBehaviour
{
    [SerializeField] private Transform respawn_point;
    [SerializeField] private int startingLives = 3;

    private int currentLives;

    private void Start()
    {
        currentLives = startingLives;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillFloor"))
        {
            currentLives--;

            if (currentLives > 0)
            {
                transform.position = respawn_point.position;
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
