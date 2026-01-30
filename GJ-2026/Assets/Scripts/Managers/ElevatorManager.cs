using System.Linq;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    private ElevatorControl elevatorControl;
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
        elevatorControl = Object.FindObjectsByType<ElevatorControl>(FindObjectsSortMode.None).Single();
        if (elevatorControl == null)
        {
            Debug.LogError("ElevatorControl not found in the scene.");
            return;
        }
        // elevatorControl.Init();
        Debug.Log("ElevatorManager initialized. opening doors.");

        elevatorControl.OpenDoors();
        // Initialization code for ElevatorManager
    }
}
