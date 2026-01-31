using System.Collections.Generic;
using UnityEngine;

public class NpcsController : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform npcParent;
    [SerializeField] private Transform player;

    [Header("Spawn Points (optional)")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Area (used if no spawn points)")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f);

    [Header("Level Scaling")]
    [SerializeField] private int minNpcs = 3;
    [SerializeField] private int maxNpcs = 12;
    [SerializeField] private int npcsPerLevel = 1;
    [SerializeField] private int baseLevel = 1;

    private readonly List<GameObject> spawnedNpcs = new List<GameObject>();

    public int CurrentLevel { get; private set; } = 1;

    public void SpawnForLevel(int level)
    {
        CurrentLevel = Mathf.Max(1, level);
        ClearNpcs();

        if (npcPrefab == null)
        {
            Debug.LogWarning("NpcsController: No NPC prefab assigned.");
            return;
        }

        int npcCount = CalculateNpcCount(CurrentLevel);
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 position = GetSpawnPosition(i);
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            GameObject npc = Instantiate(npcPrefab, position, rotation, npcParent);
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

    private int CalculateNpcCount(int level)
    {
        int levelIndex = Mathf.Max(0, level - baseLevel);
        int count = minNpcs + levelIndex * npcsPerLevel;
        return Mathf.Clamp(count, minNpcs, maxNpcs);
    }

    private Vector3 GetSpawnPosition(int index)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform point = spawnPoints[index % spawnPoints.Length];
            return point != null ? point.position : transform.position;
        }

        Vector3 half = spawnAreaSize * 0.5f;
        float x = Random.Range(-half.x, half.x);
        float z = Random.Range(-half.z, half.z);
        return spawnAreaCenter + new Vector3(x, 0f, z);
    }
}
