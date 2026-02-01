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
    [SerializeField] private Vector3 elevatorMaskLocalOffset = new Vector3(0f, 2.3f, 0.5f);
    private int currentLevel;
    private float testingTimer;
    private LevelDesignData currentDesign;
    private bool loggedSpawnOnce;
    public MaskAttributes CurrentPlayerMask { get; private set; }
    private readonly System.Collections.Generic.Dictionary<int, GameObject> elevatorMasks = new System.Collections.Generic.Dictionary<int, GameObject>();

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
        SpawnElevatorMasks(design);

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
            Debug.LogError($"GameControl failed to fade out music for elevator {elevatorName}: {ex}");
        }
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

    public void OnElevatorClosedWithPlayer(int elevatorIndex, Transform elevatorTransform)
    {
        Debug.Log($"GameControl elevator closed with player inside. Elevator index: {elevatorIndex}");
        currentLevel++;
        SpawnLevel(currentLevel);
    }

    public void OnElevatorEntered(int elevatorIndex)
    {
        if (elevatorMasks.TryGetValue(elevatorIndex, out GameObject maskObject) && maskObject != null)
        {
            Destroy(maskObject);
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
        Debug.Log($"GameControl mask selected. Shape={mask.Shape}, Eyes={mask.EyeState}, Mouth={mask.Mouth}");
    }

    private void SpawnElevatorMasks(LevelDesignData design)
    {
        if (maskSpamController == null || elevatorManager == null || design == null || design.LiftChoices == null)
        {
            return;
        }

        foreach (var pair in elevatorMasks)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value);
            }
        }
        elevatorMasks.Clear();

        ElevatorControl[] controls = elevatorManager.GetElevatorControls();
        if (controls == null)
        {
            return;
        }

        for (int i = 0; i < controls.Length; i++)
        {
            ElevatorControl control = controls[i];
            if (control == null)
            {
                continue;
            }

            ElevatorTrigger trigger = control.GetComponentInChildren<ElevatorTrigger>(true);
            if (trigger == null)
            {
                continue;
            }

            int index = trigger.ElevatorIndex;
            if (index < 0 || index >= design.LiftChoices.Count)
            {
                continue;
            }

            MaskAttributes mask = design.LiftChoices[index];
            GameObject maskObject = maskSpamController.SpawnSingleMask(mask, control.transform, control.transform.rotation, elevatorMaskLocalOffset);
            if (maskObject != null)
            {
                elevatorMasks[index] = maskObject;
            }
        }
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
