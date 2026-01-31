using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private NpcControl[] npcs;

    public void Init()
    {
        npcs = new NpcControl[0];
        npcs = Object.FindObjectsByType<NpcControl>(FindObjectsSortMode.None);
        // Initialization code for NPCs
    }

    public void TriggerRoomReaction(NpcMood reaction)
    {
        if (npcs == null || npcs.Length == 0)
        {
            Debug.LogWarning("NPCManager TriggerRoomReaction called, but no NPCs are initialized.");
            return;
        }

        Debug.Log($"NPCManager TriggerRoomReaction: prompting NPCs to react with {reaction}.");

        foreach (var npc in npcs)
        {
            try
            {
                if (npc == null)
                {
                    Debug.LogWarning("NPCManager TriggerRoomReaction skipped a missing NPC reference.");
                    continue;
                }

                npc.SetMood(reaction);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"NPCManager TriggerRoomReaction failed for an NPC: {ex.Message}");
            }
        }
    }

    public void FixedUpdate()
    {
        if (npcs != null && npcs.Length > 0)
        {
            foreach (var npc in npcs)
            {
                if (npc == null)
                {
                    Debug.LogWarning("NPCManager FixedUpdate found a missing NPC reference while checking moods.");
                    continue;
                }

                try
                {
                    _ = npc.Mood;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"NPCManager failed to read NPC mood: {ex.Message}");
                }
            }
        }
    }

}
