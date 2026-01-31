using System;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    // [Header("Submodules")]
    // TODO: private AudioManager audioManager;

    //TODO: tobi private UIManager uiManager;

    // TODO: tobi private EnvironmentManager envManager;

    // TODO: tobi private PlayerManager playerManager;

    [SerializeField] private int startLevel = 1;

    private NPCManager npcManager;
    private ElevatorManager elevatorManager;
    private LevelDesigner levelDesigner;
    private NPCSpamController npcSpamController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcManager = gameObject.AddComponent<NPCManager>();
        npcManager.Init();

        elevatorManager = gameObject.AddComponent<ElevatorManager>();
        elevatorManager.Init();

        levelDesigner = gameObject.AddComponent<LevelDesigner>();
        npcSpamController = gameObject.AddComponent<NPCSpamController>();

        LevelDesignData design = levelDesigner.GetLevelDesign(startLevel);
        npcSpamController.SpawnLevel(design);

        // load all submodules

        // UI Manager: open UI
        // Audio Manager: play background music

        // initialize whatever is needed

        Debug.Log("GameControl started.");
    }

    public void OnElevatorOccupancyChanged(bool isInside)
    {
        Debug.Log($"GameControl elevator occupancy changed. Player inside: {isInside}");
    }

    public void SetNpcReaction(NpcReactionState reaction)
    {
        Debug.Log($"GameControl requested NPC reaction change to: {reaction}");

        try
        {
            if (npcManager == null)
            {
                Debug.LogError("GameControl cannot set NPC reaction because NPCManager is missing.");
                return;
            }

            npcManager.React(reaction);
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameControl failed to set NPC reaction: {ex}");
        }
    }
}
