using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSwitcher : MonoBehaviour
{
    public Move.CycleMode cycleModeType = Move.CycleMode.FirstOnly;

    void Start() {
        var sr = GetComponent<SpriteRenderer>();
        switch (cycleModeType) {
            case Move.CycleMode.Switch: 
                sr.color = Color.white;
                break;
            case Move.CycleMode.FirstOnly: 
                sr.color = Color.cyan;
                break;
            case Move.CycleMode.SecondOnly: 
                sr.color = Color.magenta;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            collider.GetComponent<Move>().SetCycleMode(cycleModeType);
        }
    }
}
