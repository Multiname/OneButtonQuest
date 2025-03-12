using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public GameManager gameManager;
    private bool postponedCollision = false;

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            if (!gameManager.KillPlayer(false)) {
                postponedCollision = true;
                gameManager.AddCollidedSpike(this);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player") && postponedCollision) {
            gameManager.RemoveCollidedSpike(this);
            postponedCollision = false;
        }
    }

    void OnDestroy() {
        if (postponedCollision) {
            gameManager.RemoveCollidedSpike(this);
            postponedCollision = false;
        }
    }
}
