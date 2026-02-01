using System;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    private ElevatorControl[] elevatorControls;
    private ElevatorMaskDisplayController[] elevatorMaskDisplays;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init()
    {
        elevatorControls = UnityEngine.Object.FindObjectsByType<ElevatorControl>(FindObjectsSortMode.None);
        elevatorMaskDisplays = UnityEngine.Object.FindObjectsByType<ElevatorMaskDisplayController>(FindObjectsSortMode.None);
        if (elevatorControls == null || elevatorControls.Length == 0)
        {
            Debug.LogError("ElevatorManager did not find any ElevatorControls in the scene.");
            return;
        }
        if (elevatorControls.Length != 4)
        {
            Debug.LogWarning($"ElevatorManager expected 4 ElevatorControls but found {elevatorControls.Length}.");
        }

        for (int i = 0; i < elevatorControls.Length; i++)
        {
            ElevatorControl control = elevatorControls[i];
            if (control == null)
            {
                Debug.LogWarning($"ElevatorManager found a null ElevatorControl at index {i}.");
                continue;
            }

            try
            {
                control.Init();
                control.OpenDoors();
            }
            catch (Exception ex)
            {
                Debug.LogError($"ElevatorManager failed to initialize or open doors for ElevatorControl at index {i}. {ex.Message}");
                continue;
            }
        }

        Debug.Log("ElevatorManager initialized and opened doors.");
        // Initialization code for ElevatorManager
    }

    public void ResetElevators()
    {
        foreach (var elevator in elevatorControls)
        {
            if (elevator != null)
            {
                elevator.OpenDoors();
            }
        }
        
    }

    public void OpenAllExcept(int excludeIndex)
    {
        if (elevatorControls == null)
        {
            return;
        }

        foreach (var elevator in elevatorControls)
        {
            if (elevator == null)
            {
                continue;
            }

            ElevatorTrigger trigger = elevator.GetComponentInChildren<ElevatorTrigger>(true);
            int index = trigger != null ? trigger.ElevatorIndex : -1;
            if (index == excludeIndex)
            {
                continue;
            }

            elevator.OpenDoors();
        }
    }

    public void CloseByIndex(int indexToClose)
    {
        if (elevatorControls == null)
        {
            return;
        }

        foreach (var elevator in elevatorControls)
        {
            if (elevator == null)
            {
                continue;
            }

            ElevatorTrigger trigger = elevator.GetComponentInChildren<ElevatorTrigger>(true);
            int index = trigger != null ? trigger.ElevatorIndex : -1;
            if (index == indexToClose)
            {
                elevator.CloseDoors();
                return;
            }
        }
    }

    public ElevatorControl[] GetElevatorControls()
    {
        if (elevatorControls == null || elevatorControls.Length == 0)
        {
            elevatorControls = UnityEngine.Object.FindObjectsByType<ElevatorControl>(FindObjectsSortMode.None);
        }

        return elevatorControls;
    }

    public void ApplyLiftMasks(System.Collections.Generic.List<MaskAttributes> liftChoices)
    {
        if (liftChoices == null || liftChoices.Count == 0)
        {
            return;
        }

        if (elevatorMaskDisplays == null || elevatorMaskDisplays.Length == 0)
        {
            elevatorMaskDisplays = UnityEngine.Object.FindObjectsByType<ElevatorMaskDisplayController>(FindObjectsSortMode.None);
        }

        for (int i = 0; i < elevatorMaskDisplays.Length; i++)
        {
            ElevatorMaskDisplayController display = elevatorMaskDisplays[i];
            if (display == null)
            {
                continue;
            }

            int index = display.ElevatorIndex;
            if (index < 0 || index >= liftChoices.Count)
            {
                continue;
            }

            display.ApplyMask(liftChoices[index]);
        }
    }

    public void ClearLiftMask(int elevatorIndex)
    {
        if (elevatorMaskDisplays == null || elevatorMaskDisplays.Length == 0)
        {
            elevatorMaskDisplays = UnityEngine.Object.FindObjectsByType<ElevatorMaskDisplayController>(FindObjectsSortMode.None);
        }

        for (int i = 0; i < elevatorMaskDisplays.Length; i++)
        {
            ElevatorMaskDisplayController display = elevatorMaskDisplays[i];
            if (display == null)
            {
                continue;
            }

            if (display.ElevatorIndex == elevatorIndex)
            {
                display.ClearMask();
                return;
            }
        }
    }
}
