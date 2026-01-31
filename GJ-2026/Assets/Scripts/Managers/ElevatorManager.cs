using System;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    private ElevatorControl[] elevatorControls;
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
}
