using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReceiverButton : SettingsButton, IPointerDownHandler
{
    public SettingsManager.TapActions TapAction;
    public int index = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        settingsManager.SelectReceiver(this);
    }
}
