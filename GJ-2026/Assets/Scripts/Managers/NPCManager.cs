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

    // Update is called once per frame
    public void FixedUpdate()
    {
        // every second change the npcs view focus point randomly
         float dt = Time.fixedDeltaTime;
        simulationTimer += dt;
        if (simulationTimer < 3.0f)
        {
            return;
        }
        simulationTimer = 0f;
        // Debug.Log("NPCManager updating NPC view focus points.");
        foreach (NpcControl npc in npcs)
        {
            npc.LookAtPoint(new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)));

        }

    }
}
