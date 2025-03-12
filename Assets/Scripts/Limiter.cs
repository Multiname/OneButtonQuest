using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limiter : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            gameManager.KillPlayer(true);
        }
    }
}
