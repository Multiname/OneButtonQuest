using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool isRange = false;
    private GameManager gameManager;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Enemy") || 
                isRange && collider.CompareTag("EnemyWithShield") ||
                !isRange && collider.CompareTag("EnemyWithBarrier")) {
            GameObject enemy = collider.transform.parent.gameObject;
            enemy.SetActive(false);
            gameManager.resetInteractive += () => enemy.SetActive(true);
        }
    }
}
