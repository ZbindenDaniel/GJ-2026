using UnityEngine;

public class NPCManager : MonoBehaviour
{
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

                npc.ApplyReactionState(reaction);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"NPCManager TriggerRoomReaction failed for an NPC: {ex.Message}");
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

        // accoring to moods set room reactio (attack, dance, ignore, whatevaaa)
        
        // update GameControl


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
