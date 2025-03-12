using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameManager GameManager;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")) {
            GameManager.ShowFinishScreen();
        }
    }
}
