using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public List<GameObject> objectsToRemove;
    public List<GameObject> objectsToPlace;

    public GameManager gameManager;
    private SpriteRenderer sr;
    private bool activated = false;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        gameManager.resetInteractive = ResetInteractive + gameManager.resetInteractive;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (!activated) {
            if (collider.CompareTag("Player")) {
                Move move = collider.GetComponent<Move>();
                move.buttonAction = () => Activate(move);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            collider.GetComponent<Move>().buttonAction = null;
        }
    }

    void Activate(Move move) {
        foreach (GameObject obj in objectsToRemove) {
            obj.SetActive(false);
        }
        foreach (GameObject obj in objectsToPlace) {
            obj.SetActive(true);
        }

        sr.color = Color.green;
        activated = true;
        move.buttonAction = null;
    }

    void ResetInteractive() {
        foreach (GameObject obj in objectsToRemove) {
            obj.SetActive(true);
        }
        foreach (GameObject obj in objectsToPlace) {
            obj.SetActive(false);
        }

        sr.color = Color.white;
        activated = false;
    }
}
