using System.Collections;
using UnityEngine;

public class RoomEntryTrigger : MonoBehaviour
{
    [SerializeField] private float _reactionDelay = 0.5f;
    [SerializeField] private NPCManager _npcManager;
    [SerializeField] private Collider _triggerCollider;

    private bool _reactionScheduled;

    private void Reset()
    {
        _triggerCollider = GetComponent<Collider>();
        if (_triggerCollider != null)
        {
            _triggerCollider.isTrigger = true;
        }
    }

    private void OnValidate()
    {
        if (_triggerCollider == null)
        {
            _triggerCollider = GetComponent<Collider>();
        }

        if (_triggerCollider != null && !_triggerCollider.isTrigger)
        {
            _triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_reactionScheduled)
        {
            return;
        }

        if (!IsPlayer(other))
        {
            return;
        }

        Debug.Log("RoomEntryTrigger entry detected.", this);

        if (_npcManager == null)
        {
            Debug.LogWarning("RoomEntryTrigger missing NPCManager reference.", this);
            return;
        }

        _reactionScheduled = true;
        StartCoroutine(ReactAfterDelay());
    }

    private bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return true;
        }

        return other.GetComponent<PlayerController>() != null;
    }

    private IEnumerator ReactAfterDelay()
    {
        if (_reactionDelay > 0f)
        {
            yield return new WaitForSeconds(_reactionDelay);
        }
        else
        {
            yield return null;
        }

        if (_npcManager == null)
        {
            Debug.LogWarning("RoomEntryTrigger missing NPCManager reference after delay.", this);
            yield break;
        }

        Debug.Log("RoomEntryTrigger reaction started.", this);

        try
        {
            _npcManager.TriggerRoomReaction(NpcReactionState.Aggressive);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"RoomEntryTrigger failed to trigger reaction: {ex}", this);
        }
    }
}
