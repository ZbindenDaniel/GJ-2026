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
