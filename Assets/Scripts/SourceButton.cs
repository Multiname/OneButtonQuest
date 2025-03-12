using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SourceButton : SettingsButton, IPointerDownHandler
{
    public SettingsManager.PlayerActions PlayerAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        settingsManager.SelectSource(this);
    }
}
