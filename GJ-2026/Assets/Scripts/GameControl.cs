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
    [SerializeField] private bool spawnOnStart = false;
    [SerializeField] private bool testingLevelCycle = false;
    [SerializeField] private float testingLevelIntervalSeconds = 10f;

    [SerializeField] public NPCManager Npcmanager;
    [SerializeField] private LevelDesigner levelDesigner;
    [SerializeField] private NPCSpamController npcSpamController;
    [SerializeField] private ElevatorManager elevatorManager;
    [SerializeField] private MaskSpamController maskSpamController;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private MaskSelectionController maskSelectionController;
    private int currentLevel;
    private float testingTimer;
    private LevelDesignData currentDesign;
    private bool loggedSpawnOnce;

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
        if (maskSelectionController == null)
        {
            maskSelectionController = FindFirstObjectByType<MaskSelectionController>();
        }

        currentLevel = Mathf.Max(1, startLevel);
        if (spawnOnStart)
        {
            SpawnLevel(currentLevel, null);
        }

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
            SpawnLevel(currentLevel, null);
        }
    }

    private void SpawnLevel(int level, Transform elevatorTransform)
    {
        if (levelDesigner == null || npcSpamController == null)
        {
            Debug.LogWarning("GameControl SpawnLevel aborted: missing LevelDesigner or NPCSpamController.");
            return;
        }

        if (!loggedSpawnOnce)
        {
            Debug.Log($"GameControl SpawnLevel called. Level={level}.");
            loggedSpawnOnce = true;
        }

        LevelDesignData design = levelDesigner.GetLevelDesign(level);
        currentDesign = design;
        npcSpamController.SpawnLevel(design);
        if (maskSpamController != null)
        {
            if (elevatorTransform != null)
            {
                maskSpamController.SpawnMasks(design, elevatorTransform);
            }
            else
            {
                maskSpamController.SpawnMasks(design, null);
            }
        }

        if (maskSelectionController != null)
        {
            maskSelectionController.ResetSelection();
        }
        else
        {
            Debug.LogWarning("GameControl missing MaskSelectionController. Selection will not reset between levels.");
        }
    }

    public void OnElevatorOccupancyChanged(bool isInside)
    {
        Debug.Log($"GameControl elevator occupancy changed. Player inside: {isInside}");
    }

    public void OnElevatorOccupancyChanged(int elevatorIndex, bool isInside)
    {
        Debug.Log($"GameControl elevator occupancy changed. Elevator {elevatorIndex} inside: {isInside}");
        if (elevatorIndex < 0)
        {
            Debug.LogWarning("GameControl received invalid elevator index.");
            return;
        }

        if (!isInside || currentDesign == null || currentDesign.Elevators == null)
        {
            return;
        }

        for (int i = 0; i < currentDesign.Elevators.Count; i++)
        {
            ElevatorDesignData elevator = currentDesign.Elevators[i];
            if (elevator != null && elevator.Index == elevatorIndex)
            {
                Debug.Log($"GameControl elevator direction: {elevator.Direction}");
                return;
            }
        }

        Debug.LogWarning($"GameControl could not find elevator index {elevatorIndex} in current design.");
    }

    public void OnElevatorClosedWithPlayer(string elevatorName)
    {
        Debug.Log($"GameControl elevator closed with player inside. Elevator: {elevatorName}");
    }

    public void OnElevatorClosedWithPlayer(int elevatorIndex, Transform elevatorTransform)
    {
        Debug.Log($"GameControl elevator closed with player inside. Elevator index: {elevatorIndex}");
        currentLevel++;
        SpawnLevel(currentLevel, elevatorTransform);
    }

    public void OnMaskSelected(MaskAttributes mask, MaskFitType fitType)
    {
        Debug.Log($"GameControl mask selected. Shape={mask.Shape}, Eyes={mask.EyeState}, Mouth={mask.Mouth}, Fit={fitType}");
    }

    public void SetNpcReaction(NpcMood reaction)
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
