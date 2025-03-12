using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public enum TapActions {
        TapAction,
        PressAction,
        ReleaseAction,
        NoPressAction
    }

    public enum PlayerActions {
        DoNothing,
        MoveLeft,
        MoveRight,
        Jump,
        Climb,
        ActivateButton,
        AttackInMelee,
        AttackInRange,
        BecomeInvulnerable
    }

    public PlayerActions[,] Actions = new PlayerActions[2, 4];

    public List<SourceButton> SrcButtons = new() { null, null, null, null };

    private ReceiverButton selectedReceiver = null;
    private SourceButton selectedSource = null;

    public void SelectReceiver(ReceiverButton button) {
        if (selectedReceiver != null) {
            selectedReceiver.SetSelected(false);
        }
        selectedReceiver = button;
        selectedReceiver.SetSelected(true);

        if (selectedSource != null) {
            selectedSource.SetSelected(false);
        }
        selectedSource = SrcButtons[(int)Actions[button.index, (int)button.TapAction]];
        selectedSource.SetSelected(true);
    }

    public void SelectSource(SourceButton button) {
        selectedSource.SetSelected(false);
        selectedSource = button;
        selectedSource.SetSelected(true);
        Actions[selectedReceiver.index, (int)selectedReceiver.TapAction] = button.PlayerAction;
    }
}
