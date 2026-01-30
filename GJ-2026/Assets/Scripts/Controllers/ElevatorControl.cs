using System;
using UnityEngine;

public class ElevatorControl : MonoBehaviour
{
    private Transform leftDoor;
    private Vector3 leftDoorTargetPos;

    private Transform rightDoor;
    private  Vector3 rightDoorTargetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Start()
    {
        Init();
    }
    public void Init()
    {
        leftDoor = transform.Find("door L");
        rightDoor = transform.Find("door R");
        leftDoorTargetPos = leftDoor.localPosition;
        rightDoorTargetPos = rightDoor.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (leftDoor == null || rightDoor == null)
            return;
        if(leftDoor.localPosition == leftDoorTargetPos && rightDoor.localPosition == rightDoorTargetPos)
            return;
            
        leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftDoorTargetPos, Time.deltaTime * 2f);
        rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightDoorTargetPos, Time.deltaTime * 2f);
    }

    public void OpenDoors()
    {
        Debug.Log("Elevator doors opening.");
        leftDoorTargetPos = new Vector3(leftDoor.localPosition.x, leftDoor.localPosition.y, 1f);
        rightDoorTargetPos = new Vector3(rightDoor.localPosition.x, rightDoor.localPosition.y, -1f);
    }

    public void CloseDoors()
    {
        Debug.Log("Elevator doors closing.");
        leftDoorTargetPos = new Vector3(leftDoor.localPosition.x, leftDoor.localPosition.y, .5f);
        rightDoorTargetPos = new Vector3(rightDoor.localPosition.x, rightDoor.localPosition.y, -.5f);
        // Implementation for closing elevator doors
    }
}
