using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public enum RoomReaction
    {
        PlayerLooking,
        Idle,
        Neutral,
        Happy,
        Sad
    }

    private NpcControl[] npcs;
    private float simulationTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void Init()
    {
        npcs = new NpcControl[0];
        npcs = Object.FindObjectsByType<NpcControl>(FindObjectsSortMode.None);
        // Initialization code for NPCs
    }

    public void TriggerRoomReaction(RoomReaction reaction)
    {
        if (npcs == null || npcs.Length == 0)
        {
            Debug.LogWarning("NPCManager TriggerRoomReaction called, but no NPCs are initialized.");
            return;
        }

        Debug.Log($"NPCManager TriggerRoomReaction: prompting NPCs to react with {reaction}.");

        switch (reaction)
        {
            case RoomReaction.PlayerLooking:
                TriggerPlayerLooking();
                break;
            case RoomReaction.Idle:
                TriggerIdle();
                break;
            case RoomReaction.Neutral:
                TriggerNeutral();
                break;
            case RoomReaction.Happy:
                TriggerHappy();
                break;
            case RoomReaction.Sad:
                TriggerSad();
                break;
            default:
                Debug.LogWarning($"NPCManager TriggerRoomReaction received an unknown reaction: {reaction}.");
                break;
        }
    }

    private void TriggerPlayerLooking()
    {
        foreach (var npc in npcs)
        {
            try
            {
                if (npc == null)
                {
                    Debug.LogWarning("NPCManager TriggerPlayerLooking skipped a missing NPC reference.");
                    continue;
                }

                npc.LookAtPlayer();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"NPCManager TriggerPlayerLooking failed for an NPC: {ex.Message}");
            }
        }
    }

    private void TriggerIdle()
    {
        foreach (var npc in npcs)
        {
            try
            {
                if (npc == null)
                {
                    Debug.LogWarning("NPCManager TriggerIdle skipped a missing NPC reference.");
                    continue;
                }

                npc.Idle();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"NPCManager TriggerIdle failed for an NPC: {ex.Message}");
            }
        }
    }

    private void TriggerNeutral()
    {
        foreach (var npc in npcs)
        {
            try
            {
                if (npc == null)
                {
                    Debug.LogWarning("NPCManager TriggerNeutral skipped a missing NPC reference.");
                    continue;
                }

                Debug.Log("NPCManager TriggerNeutral placeholder: NPC reaction not implemented yet.");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"NPCManager TriggerNeutral failed for an NPC: {ex.Message}");
            }
        }
    }

    private void TriggerHappy()
    {
        foreach (var npc in npcs)
        {
            try
            {
                if (npc == null)
                {
                    Debug.LogWarning("NPCManager TriggerHappy skipped a missing NPC reference.");
                    continue;
                }

                Debug.Log("NPCManager TriggerHappy placeholder: NPC reaction not implemented yet.");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"NPCManager TriggerHappy failed for an NPC: {ex.Message}");
            }
        }
    }

    private void TriggerSad()
    {
        foreach (var npc in npcs)
        {
            try
            {
                if (npc == null)
                {
                    Debug.LogWarning("NPCManager TriggerSad skipped a missing NPC reference.");
                    continue;
                }

                Debug.Log("NPCManager TriggerSad placeholder: NPC reaction not implemented yet.");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"NPCManager TriggerSad failed for an NPC: {ex.Message}");
            }
        }
    }

    private int cnt = 0;

    // Update is called once per frame
    public void FixedUpdate()
    {
        // every second change the npcs view focus point randomly
        float dt = Time.fixedDeltaTime;
        simulationTimer += dt;
        if (simulationTimer < .5f)
        {
            return;
        }
        cnt++;
        simulationTimer = 0f;


        // Look randomly around for 10 cycles, then look at player for 4 cycles, then idle

        if (cnt == 20)
        {
            foreach (var npcs in npcs)
            {
                if (npcs != null)
                    npcs.LookAtPlayer();
            }
            return;
        }
        if (cnt == 30)
        {
            foreach (var npcs in npcs)
            {
                if (npcs != null)
                    npcs.Idle();
            }
            return;
        }

        if (cnt < 20)
        {


            // Debug.Log("NPCManager updating NPC view focus points.");
            var randomNpc = Random.Range(0, npcs.Length);
            var npc = npcs[randomNpc];
            if (npc != null)
                npc.LookAtPoint(new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)));
        }

    }

}
