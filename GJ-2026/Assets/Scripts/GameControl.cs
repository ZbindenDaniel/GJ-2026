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
    [SerializeField] private bool testingLevelCycle = false;
    [SerializeField] private float testingLevelIntervalSeconds = 10f;

    private NPCManager npcManager;
    private ElevatorManager elevatorManager;
    private LevelDesigner levelDesigner;
    private NPCSpamController npcSpamController;
    private int currentLevel;
    private float testingTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcManager = gameObject.AddComponent<NPCManager>();
        npcManager.Init();

        elevatorManager = gameObject.AddComponent<ElevatorManager>();
        elevatorManager.Init();

        levelDesigner = gameObject.AddComponent<LevelDesigner>();
        npcSpamController = gameObject.AddComponent<NPCSpamController>();

        currentLevel = Mathf.Max(1, startLevel);
        SpawnLevel(currentLevel);

        // load all submodules

        // UI Manager: open UI
        // Audio Manager: play background music

        // initialize whatever is needed

        Debug.Log("GameControl started.");
    }

    void Update()
    {
        if (!testingLevelCycle)
        {
            return;
        }

        testingTimer += Time.deltaTime;
        if (testingTimer >= testingLevelIntervalSeconds)
        {
            testingTimer = 0f;
            currentLevel++;
            SpawnLevel(currentLevel);
        }
    }

    private void SpawnLevel(int level)
    {
        if (levelDesigner == null || npcSpamController == null)
        {
            return;
        }

        LevelDesignData design = levelDesigner.GetLevelDesign(level);
        npcSpamController.SpawnLevel(design);
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
