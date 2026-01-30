using System;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    // [Header("Submodules")]
    // TODO: private AudioManager audioManager;

    //TODO: tobi private UIManager uiManager;

    // TODO: tobi private EnvironmentManager envManager;

    // TODO: tobi private PlayerManager playerManager;

    private NPCManager npcManager;

    private ElevatorManager elevatorManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcManager = gameObject.AddComponent<NPCManager>();
        npcManager.Init();

        elevatorManager = gameObject.AddComponent<ElevatorManager>();
        elevatorManager.Init();

        // load all submodules

        // UI Manager: open UI
        // Audio Manager: play background music

        // initialize whatever is needed

        Debug.Log("GameControl started.");
    }
}
