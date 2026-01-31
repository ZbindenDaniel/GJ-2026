using System.Collections.Generic;
using UnityEngine;

public class NPCSpamController : MonoBehaviour
{
    // Static tuning knobs (fast jam tweaks)
    public static Vector3 LevelSize = new Vector3(8f, 0f, 8f);
    public static float MinNpcDistance = 1.5f;
    public static int MaxAttemptsPerNpc = 25;
    public static Vector3 LevelCenter = Vector3.zero;
    public static float GroundY = 0f;

    [Header("Prefab")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform npcParent;
    [SerializeField] private Transform player;

    private readonly List<GameObject> spawnedNpcs = new List<GameObject>();
    private bool loggedOnce;

    public void SpawnLevel(LevelDesignData design)
    {
        if (!loggedOnce)
        {
            Debug.Log("NPCSpamController SpawnLevel called.");
            loggedOnce = true;
        }
        if (design == null)
        {
            Debug.LogWarning("NPCSpamController: Missing level design data.");
            return;
        }
        if (npcPrefab == null)
        {
            Debug.LogWarning("NPCSpamController: No NPC prefab assigned.");
            return;
        }

        ClearNpcs();

        List<Vector3> positions = GeneratePositions(design.NpcCount);
        for (int i = 0; i < positions.Count; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            GameObject npc = Instantiate(npcPrefab, positions[i], rotation, npcParent);

            // scale the height (Y) randomly between 0.8 and 1.2
            float scale = Random.Range(0.8f, 1.2f);
            npc.transform.localScale = new Vector3(1f, scale, 1f);

            if (!npc.activeSelf)
            {
                npc.SetActive(true);
                npc.GetComponent<NpcControl>().LookAtPlayer();
            }
            spawnedNpcs.Add(npc);

            NpcControl npcControl = npc.GetComponent<NpcControl>();
            if (npcControl != null && player != null)
            {
                npcControl.player = player;
            }
        }
    }

    public void ClearNpcs()
    {
        for (int i = 0; i < spawnedNpcs.Count; i++)
        {
            if (spawnedNpcs[i] != null)
            {
                Destroy(spawnedNpcs[i]);
            }
        }
        spawnedNpcs.Clear();
    }

    private static List<Vector3> GeneratePositions(int count)
    {
        List<Vector3> positions = new List<Vector3>(count);
        Vector3 half = LevelSize * 0.5f;

        for (int i = 0; i < count; i++)
        {
            bool placed = false;
            for (int attempt = 0; attempt < MaxAttemptsPerNpc; attempt++)
            {
                float x = Random.Range(-half.x, half.x);
                float z = Random.Range(-half.z, half.z);
                Vector3 pos = new Vector3(LevelCenter.x + x, GroundY, LevelCenter.z + z);

                if (IsFarEnough(pos, positions))
                {
                    positions.Add(pos);
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                positions.Add(new Vector3(LevelCenter.x, GroundY, LevelCenter.z));
            }
        }

        return positions;
    }

    private static bool IsFarEnough(Vector3 position, List<Vector3> existing)
    {
        for (int i = 0; i < existing.Count; i++)
        {
            if (Vector3.Distance(position, existing[i]) < MinNpcDistance)
            {
                return false;
            }
        }
        return true;
    }
}
