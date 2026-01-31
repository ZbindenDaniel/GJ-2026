using System.Collections.Generic;
using UnityEngine;

public class NPCSpamController : MonoBehaviour
{
    // Static tuning knobs (fast jam tweaks)
    public static Vector3 LevelSize = new Vector3(10f, 0f, 10f);
    public static float MinNpcDistance = 1.5f;
    public static int MaxAttemptsPerNpc = 25;
    public static Vector3 LevelCenter = Vector3.zero;
    public static float GroundY = 0f;
    public static int MinGroupSize = 3;
    public static int MaxGroupSize = 5;
    public static float GroupRadiusMin = 0.6f;
    public static float GroupRadiusMax = 1.4f;
    public static int MaxAttemptsPerGroup = 15;

    [Header("Prefab")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform npcParent;
    [SerializeField] private Transform player;

    private readonly List<GameObject> spawnedNpcs = new List<GameObject>();
    private struct SpawnPoint
    {
        public Vector3 Position;
        public Vector3 LookAt;
    }

    public void SpawnLevel(LevelDesignData design)
    {
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

        List<SpawnPoint> spawnPoints = GeneratePositions(design.NpcCount);
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            try
            {
                Vector3 direction = spawnPoints[i].LookAt - spawnPoints[i].Position;
                Quaternion rotation = direction.sqrMagnitude > 0.001f
                    ? Quaternion.LookRotation(direction, Vector3.up)
                    : Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                GameObject npc = Instantiate(npcPrefab, spawnPoints[i].Position, rotation, npcParent);
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
            catch (System.Exception ex)
            {
                Debug.LogError($"NPCSpamController: Failed to spawn NPC {i}. {ex.Message}");
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

    private static List<SpawnPoint> GeneratePositions(int count)
    {
        List<SpawnPoint> positions = new List<SpawnPoint>(count);
        Vector3 half = LevelSize * 0.5f;
        int remaining = count;

        while (remaining > 0)
        {
            int groupSize = Mathf.Clamp(Random.Range(MinGroupSize, MaxGroupSize + 1), 1, remaining);
            bool placedGroup = false;

            for (int groupAttempt = 0; groupAttempt < MaxAttemptsPerGroup; groupAttempt++)
            {
                float centerX = Random.Range(-half.x, half.x);
                float centerZ = Random.Range(-half.z, half.z);
                Vector3 groupCenter = new Vector3(LevelCenter.x + centerX, GroundY, LevelCenter.z + centerZ);
                if (!IsInsideLevel(groupCenter, half))
                {
                    continue;
                }

                List<SpawnPoint> groupPoints = new List<SpawnPoint>(groupSize);
                bool groupValid = true;
                for (int i = 0; i < groupSize; i++)
                {
                    float angle = (360f / groupSize) * i + Random.Range(-12f, 12f);
                    float radius = Random.Range(GroupRadiusMin, GroupRadiusMax);
                    float rad = angle * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(rad) * radius, 0f, Mathf.Sin(rad) * radius);
                    Vector3 pos = groupCenter + offset;

                    if (!IsInsideLevel(pos, half) || !IsFarEnough(pos, positions, groupPoints))
                    {
                        groupValid = false;
                        break;
                    }

                    groupPoints.Add(new SpawnPoint
                    {
                        Position = pos,
                        LookAt = groupCenter
                    });
                }

                if (groupValid)
                {
                    positions.AddRange(groupPoints);
                    remaining -= groupPoints.Count;
                    placedGroup = true;
                    break;
                }
            }

            if (!placedGroup)
            {
                Debug.LogWarning("NPCSpamController: Failed to place group, falling back to single NPC placement.");
                SpawnPoint fallback = GenerateSinglePosition(positions, half);
                positions.Add(fallback);
                remaining -= 1;
            }
        }

        return positions;
    }

    private static SpawnPoint GenerateSinglePosition(List<SpawnPoint> existing, Vector3 half)
    {
        for (int attempt = 0; attempt < MaxAttemptsPerNpc; attempt++)
        {
            float x = Random.Range(-half.x, half.x);
            float z = Random.Range(-half.z, half.z);
            Vector3 pos = new Vector3(LevelCenter.x + x, GroundY, LevelCenter.z + z);

            if (IsFarEnough(pos, existing, null))
            {
                return new SpawnPoint
                {
                    Position = pos,
                    LookAt = pos
                };
            }
        }

        Vector3 fallbackPos = new Vector3(LevelCenter.x, GroundY, LevelCenter.z);
        return new SpawnPoint
        {
            Position = fallbackPos,
            LookAt = fallbackPos
        };
    }

    private static bool IsInsideLevel(Vector3 position, Vector3 half)
    {
        return position.x >= LevelCenter.x - half.x && position.x <= LevelCenter.x + half.x
            && position.z >= LevelCenter.z - half.z && position.z <= LevelCenter.z + half.z;
    }

    private static bool IsFarEnough(Vector3 position, List<SpawnPoint> existing, List<SpawnPoint> pending)
    {
        for (int i = 0; i < existing.Count; i++)
        {
            if (Vector3.Distance(position, existing[i].Position) < MinNpcDistance)
            {
                return false;
            }
        }
        if (pending != null)
        {
            for (int i = 0; i < pending.Count; i++)
            {
                if (Vector3.Distance(position, pending[i].Position) < MinNpcDistance)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
