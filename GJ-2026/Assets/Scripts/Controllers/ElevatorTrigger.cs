using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ElevatorTrigger : MonoBehaviour
{
    [SerializeField] private ElevatorControl _elevatorControl;
    [SerializeField] private GameControl _gameControl;
    [SerializeField] private int _elevatorIndex = -1;
    [SerializeField] private float _closeDoorsDelay = 1.5f;
    [SerializeField] private float _reopenDoorsDelay = 1.5f;

    private bool isPlayerInside;
    private Coroutine closeDoorsRoutine;
    private Coroutine reopenDoorsRoutine;
    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            Debug.LogWarning($"ElevatorTrigger expects a trigger collider on {name}.");
        }

        if (_elevatorControl == null)
        {
            _elevatorControl = GetComponentInParent<ElevatorControl>();
        }

        if (_gameControl == null)
        {
            _gameControl = FindFirstObjectByType<GameControl>();
        }
    }

    private void OnDisable()
    {
        if (closeDoorsRoutine != null)
        {
            StopCoroutine(closeDoorsRoutine);
            closeDoorsRoutine = null;
        }

        if (reopenDoorsRoutine != null)
        {
            StopCoroutine(reopenDoorsRoutine);
            reopenDoorsRoutine = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCollider(other))
        {
            return;
        }

        if (isPlayerInside)
        {
            return;
        }

        isPlayerInside = true;
        Debug.Log($"Player entered elevator trigger for {GetElevatorName()}.");
        NotifyGameControl(true);
        ScheduleCloseDoors();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayerCollider(other))
        {
            return;
        }

        if (!isPlayerInside)
        {
            return;
        }

        isPlayerInside = false;
        Debug.Log($"Player exited elevator trigger for {GetElevatorName()}.");
        NotifyGameControl(false);
        ScheduleCloseDoors();
    }

    private bool IsPlayerCollider(Collider other)
    {
        if (other == null)
        {
            return false;
        }

        if (other.CompareTag("Player"))
        {
            return true;
        }

        return other.GetComponentInParent<PlayerController>() != null;
    }

    private void ScheduleCloseDoors()
    {
        if (_elevatorControl == null)
        {
            Debug.LogWarning("ElevatorTrigger missing ElevatorControl reference.");
            return;
        }

        if (closeDoorsRoutine != null)
        {
            StopCoroutine(closeDoorsRoutine);
        }

        closeDoorsRoutine = StartCoroutine(CloseDoorsAfterDelay());
    }

    private IEnumerator CloseDoorsAfterDelay()
    {
        yield return new WaitForSeconds(_closeDoorsDelay);
        try
        {
            _elevatorControl.CloseDoors();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to close elevator doors for {GetElevatorName()}: {ex}");
        }

        if (isPlayerInside)
        {
            try
            {
                _elevatorControl.PlayElevetorMusic();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to start elevator music for {GetElevatorName()}: {ex}");
            }

            NotifyGameControlElevatorClosedWithPlayer(GetElevatorName());
            ScheduleOpenDoors();
        }
        closeDoorsRoutine = null;
    }

    private void ScheduleOpenDoors()
    {
        if (_elevatorControl == null)
        {
            Debug.LogWarning("ElevatorTrigger missing ElevatorControl reference for reopening.");
            return;
        }

        if (reopenDoorsRoutine != null)
        {
            StopCoroutine(reopenDoorsRoutine);
        }

        reopenDoorsRoutine = StartCoroutine(OpenDoorsAfterDelay());
    }

    private IEnumerator OpenDoorsAfterDelay()
    {
        yield return new WaitForSeconds(_reopenDoorsDelay);
        try
        {
            _elevatorControl.OpenDoors();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to open elevator doors for {GetElevatorName()}: {ex}");
        }

        if (isPlayerInside)
        {
            try
            {
                _elevatorControl.StopElevatorMusic();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to stop elevator music for {GetElevatorName()}: {ex}");
            }

            NotifyGameControlElevatorOpenedWithPlayer(GetElevatorName());
        }

        reopenDoorsRoutine = null;
    }

    private void NotifyGameControl(bool isInside)
    {
        if (_gameControl == null)
        {
            Debug.LogWarning("ElevatorTrigger could not find GameControl to notify.");
            return;
        }

        try
        {
            if (_elevatorIndex >= 0)
            {
                _gameControl.OnElevatorOccupancyChanged(_elevatorIndex, isInside);
            }
            else
            {
                _gameControl.OnElevatorOccupancyChanged(isInside);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to notify GameControl about elevator occupancy: {ex}");
        }
    }

    private void NotifyGameControlElevatorClosedWithPlayer(string elevatorName)
    {
        if (_gameControl == null)
        {
            Debug.LogWarning("ElevatorTrigger could not find GameControl to notify about elevator closing.");
            return;
        }

        try
        {
            _gameControl.OnElevatorClosedWithPlayer(elevatorName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to notify GameControl about elevator closing with player ({elevatorName}): {ex}");
        }
    }

    private void NotifyGameControlElevatorOpenedWithPlayer(string elevatorName)
    {
        if (_gameControl == null)
        {
            Debug.LogWarning("ElevatorTrigger could not find GameControl to notify about elevator opening.");
            return;
        }

        try
        {
            _gameControl.OnElevatorOpenedWithPlayer(elevatorName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to notify GameControl about elevator opening with player ({elevatorName}): {ex}");
        }
    }

    private string GetElevatorName()
    {
        if (_elevatorControl != null)
        {
            return _elevatorControl.name;
        }

        return name;
    }
}
