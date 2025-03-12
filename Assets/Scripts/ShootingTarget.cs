using System.Collections.Generic;
using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    public List<GameObject> objectsToRemove;
    public List<GameObject> objectsToPlace;

    public GameManager gameManager;
    private SpriteRenderer sr;
    private bool activated = false;
    private Color baseColor;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        gameManager.resetInteractive = ResetInteractive + gameManager.resetInteractive;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (!activated) {
            if (collider.CompareTag("Projectile")) {
                foreach (GameObject obj in objectsToRemove) {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in objectsToPlace) {
                    obj.SetActive(true);
                }

                sr.color = Color.green;
                activated = true;
                tag = "Untagged";
            }
        }
    }

    void ResetInteractive() {
        foreach (GameObject obj in objectsToRemove) {
            obj.SetActive(true);
        }
        foreach (GameObject obj in objectsToPlace) {
            obj.SetActive(false);
        }

        sr.color = baseColor;
        activated = false;
        tag = "ShootingTarget";
    }
}
