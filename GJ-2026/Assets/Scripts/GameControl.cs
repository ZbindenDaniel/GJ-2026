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

    [SerializeField] public NPCManager Npcmanager;
    [SerializeField] private LevelDesigner levelDesigner;
    [SerializeField] private NPCSpamController npcSpamController;
    [SerializeField] private ElevatorManager elevatorManager;
    [SerializeField] private MaskSpamController maskSpamController;
    private int currentLevel;
    private float testingTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Npcmanager == null)
        {
            Debug.Log("GameControl missing NPCManager.");
            return;
        }
        Npcmanager.Init();

        if (elevatorManager == null)
        {
            Debug.Log("GameControl missing ElevatorManager.");
            return;
        }

        elevatorManager.Init();

        if (levelDesigner == null)
        {
            levelDesigner = GetComponent<LevelDesigner>();
        }
        if (npcSpamController == null)
        {
            npcSpamController = GetComponent<NPCSpamController>();
        }
        if (maskSpamController == null)
        {
            maskSpamController = GetComponent<MaskSpamController>();
        }

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
        if (maskSpamController != null)
        {
            maskSpamController.SpawnMasks(design);
        }
    }

    public void OnElevatorOccupancyChanged(bool isInside)
    {
        Debug.Log($"GameControl elevator occupancy changed. Player inside: {isInside}");
    }

    public void OnElevatorClosedWithPlayer()
    {
        Debug.Log("GameControl elevator closed with player inside.");
    }

    public void SetNpcReaction(NpcReactionState reaction)
    {
        Debug.Log($"GameControl requested NPC reaction change to: {reaction}");

        try
        {
            if (Npcmanager == null)
            {
                Debug.LogError("GameControl cannot set NPC reaction because NPCManager is missing.");
                return;
            }

            Npcmanager.TriggerRoomReaction(reaction);
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameControl failed to set NPC reaction: {ex}");
        }
    }
}
