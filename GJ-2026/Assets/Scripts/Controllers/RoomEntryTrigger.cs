using System.Collections;
using UnityEngine;

public class RoomEntryTrigger : MonoBehaviour
{
    [SerializeField] public GameControl GameControl;
    [SerializeField] private float _reactionDelay = 0.5f;
    [SerializeField] private Collider _triggerCollider;
    [SerializeField] private NpcReactionState _reaction = NpcReactionState.Idle;

    private NPCManager _npcManager;
    private bool _reactionScheduled;


    void Start()
    {
        try
        {
            if (GameControl == null)
            {
                GameControl = FindFirstObjectByType<GameControl>();
                if (GameControl == null)
                {
                    Debug.LogWarning("RoomEntryTrigger missing GameControl reference; NPC reaction setup skipped.", this);
                    return;
                }

                Debug.LogWarning("RoomEntryTrigger GameControl reference was missing; using scene lookup fallback.", this);
            }

            _npcManager = GameControl.Npcmanager;
            if (_npcManager == null)
            {
                Debug.LogWarning("RoomEntryTrigger missing NPCManager reference from GameControl.", this);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"RoomEntryTrigger failed during Start setup: {ex}", this);
        }
    }
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
            Debug.Log($"RoomEntryTrigger triggering NPC reaction: {_reaction}.", this);
            _npcManager.TriggerRoomReaction(_reaction);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"RoomEntryTrigger failed to trigger reaction: {ex}", this);
        }
    }
}
