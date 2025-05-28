using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlat : MonoBehaviour
{
    public string playerTag = "Player";

    private float timer = 0f;
    private bool playerOnTop = false;

    void Update()
    {
        if (playerOnTop)
        {
            timer += Time.deltaTime;
            if (timer >= 0.3f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            timer = 0f; // reset timer if player steps off
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            playerOnTop = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            playerOnTop = false;
        }
    }
}