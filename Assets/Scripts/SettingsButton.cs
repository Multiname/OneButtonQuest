using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    public SettingsManager settingsManager;

    private Image image;

    void Start() {
        image = GetComponent<Image>();
    }

    public void SetSelected(bool selected) {
        if (selected) {
            image.color = Color.green;
        } else {
            image.color = Color.white;
        }
    }
}
