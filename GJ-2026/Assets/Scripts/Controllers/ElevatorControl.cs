using System;
using UnityEngine;

public class ElevatorControl : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _elevatorMusic;

    private Transform leftDoor;
    private Vector3 leftDoorTargetPos;

    private Transform rightDoor;
    private  Vector3 rightDoorTargetPos;

    private void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (_audioSource == null)
        {
            Debug.LogWarning("ElevatorControl is missing an AudioSource reference.", this);
        }
    }

    public void Start()
    {
        // elevatorCollider = GetComponent<Collider>();
    }

    public void Init()
    {
        leftDoor = transform.GetChild(0).Find("LeftElevatorDoor");
        rightDoor = transform.GetChild(0).Find("RightElevatorDoor");
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
        leftDoorTargetPos = new Vector3(0f, 0f, .5f);
        rightDoorTargetPos = new Vector3(0f, 0f, -.5f);
    }

    public void CloseDoors()
    {
        Debug.Log("Elevator doors closing.");
        leftDoorTargetPos = new Vector3(0f, 0f, .01f);
        rightDoorTargetPos = new Vector3(0f, 0f, -.01f);

        // TODO: only if player inside elevator play music

        PlayElevetorMusic();
    }

    public void PlayElevetorMusic()
    {
        try
        {
            if (_audioSource == null)
            {
                Debug.LogWarning("ElevatorControl cannot play elevator music because no AudioSource is assigned.", this);
                return;
            }

            if (_elevatorMusic == null)
            {
                Debug.LogWarning("ElevatorControl has no elevator music configured.", this);
                return;
            }

            _audioSource.PlayOneShot(_elevatorMusic);
        }
        catch (Exception exception)
        {
            Debug.LogError($"ElevatorControl failed to play elevator music: {exception}", this);
        }
    }
}
