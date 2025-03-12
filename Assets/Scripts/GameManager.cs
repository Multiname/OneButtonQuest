using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public delegate void ResetInteractive();
    public ResetInteractive resetInteractive = null;

    public GameObject Player;
    public GameObject Canvas;
    public GameObject Settings;
    public GameObject RestartButton;
    public SettingsManager settingsManager;
    public GameObject FinishScreen;
    private Vector2 PlayerStartPosition;

    public int nextSceneIndex;
    
    private Move moveScript;

    private List<Spike> collidedSpikes = new();

    void Start() {
        moveScript = Player.GetComponent<Move>();
        PlayerStartPosition = Player.transform.position;
        moveScript.handleInvulnerabilityDisabling = CheckPostponedKilling;
        Canvas.SetActive(true);
    }

    public void StartGame() {
        moveScript.enabled = true;
        moveScript.SetActions(
            new int[]{
                (int)settingsManager.Actions[0, (int)SettingsManager.TapActions.TapAction],
                (int)settingsManager.Actions[1, (int)SettingsManager.TapActions.TapAction]
            }, 
            new int[]{
                (int)settingsManager.Actions[0, (int)SettingsManager.TapActions.PressAction],
                (int)settingsManager.Actions[1, (int)SettingsManager.TapActions.PressAction]
            }, 
            new int[]{
                (int)settingsManager.Actions[0, (int)SettingsManager.TapActions.ReleaseAction],
                (int)settingsManager.Actions[1, (int)SettingsManager.TapActions.ReleaseAction]
            }, 
            new int[]{
                (int)settingsManager.Actions[0, (int)SettingsManager.TapActions.NoPressAction],
                (int)settingsManager.Actions[1, (int)SettingsManager.TapActions.NoPressAction]
            }
        );
        Settings.SetActive(false);
        RestartButton.SetActive(true);
    }

    public void RestartGame() {
        RestartButton.SetActive(false);
        Settings.SetActive(true);
        moveScript.enabled = false;
        Player.transform.position = PlayerStartPosition;
        resetInteractive?.Invoke();
        foreach (var obj in GameObject.FindGameObjectsWithTag("Projectile")) {
            Destroy(obj);
        }
        foreach (var obj in GameObject.FindGameObjectsWithTag("HostileProjectile")) {
            Destroy(obj);
        }
    }

    public bool KillPlayer(bool ignoreInvulnerability) {
        if (!moveScript.invulnerable || ignoreInvulnerability) {
            moveScript.Die();
            Player.transform.position = PlayerStartPosition;
            return true;
        }
        return false;
    }

    public void AddCollidedSpike(Spike spike) {
        collidedSpikes.Add(spike);
    }

    public void RemoveCollidedSpike(Spike spike) {
        collidedSpikes.Remove(spike);
    }

    public void CheckPostponedKilling() {
        if (collidedSpikes.Count > 0) {
            KillPlayer(false);
        }
    }

    public void ShowFinishScreen() {
        RestartButton.SetActive(false);
        moveScript.enabled = false;
        FinishScreen.SetActive(true);
    }

    public void LoadNextLevel() {
        SceneManager.LoadScene(nextSceneIndex);
    }
}
