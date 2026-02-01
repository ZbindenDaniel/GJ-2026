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
    [SerializeField] private bool enableMaskSpawning = false;
    private int currentLevel;
    private float testingTimer;
    private LevelDesignData currentDesign;
    private bool loggedSpawnOnce;
    public MaskAttributes CurrentPlayerMask { get; private set; }

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

        // load all submoelevatorTransformdules

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
        CurrentPlayerMask = design.PlayerMask;
        musicManager.PlayFloorSound(design.LevelIndex);
        elevatorManager.ResetElevators();
        elevatorManager.ApplyLiftMasks(design.LiftChoices);

        if (maskSelectionController != null)
        {
            maskSelectionController.ResetSelection();
        }
        else
        {
            Debug.LogWarning("GameControl missing MaskSelectionController. Selection will not reset between levels.");
        }
    }

    public void OnElevatorOccupancyChanged(int elevatorIndex, bool isInside)
    {
        // Debug.Log($"GameControl elevator occupancy changed. Elevator {elevatorIndex} inside: {isInside}");
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

    public void OnElevatorOpenedWithPlayer(string elevatorName)
    {
        Debug.Log($"GameControl elevator opened with player inside. Elevator: {elevatorName}");
        try
        {
            if (musicManager == null)
            {
                Debug.LogWarning("GameControl cannot fade in music because MusicManager is missing.");
                return;
            }

            SpawnLevel(currentLevel++);
            musicManager.FadeIn();
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameControl failed to fade in music for elevator {elevatorName}: {ex}");
        }
    }

    public void OnElevatorClosedWithPlayer(int elevatorIndex)
    {
        try
        {
            if (musicManager == null)
            {
                Debug.LogWarning("GameControl cannot fade out music because MusicManager is missing.");
                return;
            }

            musicManager.FadeOut();
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameControl failed to fade out music for elevator {elevatorIndex}: {ex}");
        }

        // Debug.Log($"GameControl elevator closed with player inside. Elevator index: {elevatorIndex}");

        // elevator check goes here
        if (currentDesign == null)
        {
            Debug.LogWarning("GameControl cannot process elevator close: missing current level design.");
            return;
        }
        if (elevatorIndex == currentDesign.TargetElevatorIndex)
        {
            Debug.Log("GameControl: Player entered the correct elevator.");
            SetNpcReaction(NpcMood.Happy);
            SpawnLevel(currentLevel++);
        }
        else
        {
            Debug.Log("GameControl: Player entered the wrong elevator.");
            SetNpcReaction(NpcMood.Assault);
            SpawnLevel(0);
                
            }

    }

    public void OnElevatorEntered(int elevatorIndex)
    {
        if (elevatorManager != null)
        {
            elevatorManager.ClearLiftMask(elevatorIndex);
        }
    }

    public void OnElevatorExited(int elevatorIndex)
    {
        if (elevatorManager == null)
        {
            return;
        }

        elevatorManager.CloseByIndex(elevatorIndex);
        elevatorManager.OpenAllExcept(elevatorIndex);
    }

    public void OnMaskSelected(MaskAttributes mask)
    {
        // TODO: tobi apply mask to player character maybe here the best place??
        Debug.Log($"GameControl mask selected. Shape={mask.Shape}, Eyes={mask.EyeState}, Mouth={mask.Mouth}");
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
