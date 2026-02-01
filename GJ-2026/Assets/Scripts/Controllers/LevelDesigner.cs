using System.Collections.Generic;
using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    // Static tuning knobs (fast jam tweaks)
    public static int MinNpcs = 3;
    public static int MaxNpcs = 12;
    public static int NpcsPerLevel = 1;
    public static int BaseLevel = 1;

    public static int MinMasks = 3;
    public static int MaxMasks = 8;
    public static int MasksPerLevel = 1;
    public static int ElevatorCount = 4;
    public static bool ShuffleElevators = true;
    public static int DefaultPlayerElevatorIndex = 1;
    private bool _loggedOnce;

    public LevelDesignData GetLevelDesign(int level)
    {
        if (!_loggedOnce)
        {
            Debug.Log($"LevelDesigner called. Level={level}.");
            _loggedOnce = true;
        }
        int safeLevel = Mathf.Max(1, level);
        int attributeCount = GetAttributeCount(safeLevel);
        int npcCount = CalculateNpcCount(safeLevel);

        LevelDesignData design = new LevelDesignData
        {
            LevelIndex = safeLevel,
            NpcCount = npcCount,
            AttributeCount = attributeCount,
            Npcs = new List<NpcDesignData>(npcCount),
            Elevators = new List<ElevatorDesignData>(),
            TargetElevatorIndex = 0,
            LiftChoices = new List<MaskAttributes>()
        };

        for (int i = 0; i < npcCount; i++)
        {
            NpcDesignData npc = new NpcDesignData
            {
                Id = i,
                Mask = CreateMaskAttributes(attributeCount)
            };
            design.Npcs.Add(npc);
        }

        design.PlayerMask = PickPlayerMask(design.Npcs, attributeCount);
        design.LiftChoices = CreateLiftChoices(design.PlayerMask, attributeCount);
        design.Elevators = CreateElevators();

        Debug.Log("TODO: here we need to set the target elevator index");
        design.TargetElevatorIndex = 2; //ayerElevatorIndex(design.Elevators.Count);
        return design;
    }

    private static int CalculateNpcCount(int level)
    {
        int levelIndex = Mathf.Max(0, level - BaseLevel);
        int count = MinNpcs + levelIndex * NpcsPerLevel;
        return Mathf.Clamp(count, MinNpcs, MaxNpcs);
    }

    private static int GetAttributeCount(int level)
    {
        if (level <= 2)
        {
            return 1;
        }
        return 2;
    }

    private static int CalculateMaskCount(int level)
    {
        int levelIndex = Mathf.Max(0, level - BaseLevel);
        int count = MinMasks + levelIndex * MasksPerLevel;
        return Mathf.Clamp(count, MinMasks, MaxMasks);
    }

    private static MaskAttributes CreateMaskAttributes(int attributeCount)
    {
        MaskAttributes mask = new MaskAttributes
        {
            Shape = GetRandomShape(),
            EyeState = GetRandomEyeState(),
            Mouth = GetRandomMouthMood()
        };

        // Only allow both eyes + mouth or none at all.
        if (attributeCount < 2)
        {
            mask.EyeState = EyeState.None;
            mask.Mouth = MouthMood.None;
        }

        return mask;
    }

    private static List<ElevatorDesignData> CreateElevators()
    {
        List<ElevatorDesignData> elevators = new List<ElevatorDesignData>();
        if (ElevatorCount <= 0)
        {
            return elevators;
        }

        int playerIndex = GetPlayerElevatorIndex(ElevatorCount);
        List<int> directions = new List<int> { -1, 0, 1 };
        while (directions.Count < ElevatorCount - 1)
        {
            directions.Add(0);
        }

        if (ShuffleElevators)
        {
            for (int i = directions.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Range(0, i + 1);
                int temp = directions[i];
                directions[i] = directions[swapIndex];
                directions[swapIndex] = temp;
            }
        }

        int directionIndex = 0;
        for (int i = 0; i < ElevatorCount; i++)
        {
            int direction = i == playerIndex ? 2 : directions[directionIndex++ % directions.Count];
            elevators.Add(new ElevatorDesignData
            {
                Index = i,
                Direction = direction
            });
        }

        return elevators;
    }

    private static int GetPlayerElevatorIndex(int elevatorCount)
    {
        if (elevatorCount <= 0)
        {
            return 0;
        }

        int index = Mathf.Clamp(DefaultPlayerElevatorIndex, 0, elevatorCount - 1);
        return index;
    }

    private static MaskAttributes PickPlayerMask(List<NpcDesignData> npcs, int attributeCount)
    {
        if (npcs != null && npcs.Count > 0)
        {
            int index = Random.Range(0, npcs.Count);
            return NormalizeMask(npcs[index].Mask, attributeCount);
        }

        return CreateMaskAttributes(attributeCount);
    }

    private static List<MaskAttributes> CreateLiftChoices(MaskAttributes playerMask, int attributeCount)
    {
        List<MaskAttributes> choices = new List<MaskAttributes>(3);
        HashSet<MaskAttributes> used = new HashSet<MaskAttributes>();

        MaskAttributes normalizedPlayer = NormalizeMask(playerMask, attributeCount);
        choices.Add(normalizedPlayer);
        used.Add(normalizedPlayer);

        if (attributeCount < 2)
        {
            MaskAttributes alt1 = normalizedPlayer;
            alt1.Shape = GetDifferentShape(normalizedPlayer.Shape);
            choices.Add(alt1);
            used.Add(alt1);

            MaskAttributes alt2 = alt1;
            alt2.Shape = GetDifferentShape(alt1.Shape);
            if (!used.Contains(alt2))
            {
                choices.Add(alt2);
            }
            else
            {
                choices.Add(normalizedPlayer);
            }
        }
        else
        {
            MaskAttributes decoy1 = CreateDecoy(normalizedPlayer, 1);
            MaskAttributes decoy2 = CreateDecoy(normalizedPlayer, 2);

            if (used.Add(decoy1))
            {
                choices.Add(decoy1);
            }
            if (choices.Count < 3 && used.Add(decoy2))
            {
                choices.Add(decoy2);
            }

            while (choices.Count < 3)
            {
                MaskAttributes extra = CreateDecoy(normalizedPlayer, Random.Range(1, 3));
                if (used.Add(extra))
                {
                    choices.Add(extra);
                }
            }
        }

        // Shuffle
        for (int i = choices.Count - 1; i > 0; i--)
        {
            int swap = Random.Range(0, i + 1);
            MaskAttributes temp = choices[i];
            choices[i] = choices[swap];
            choices[swap] = temp;
        }

        return choices;
    }

    private static MaskAttributes CreateDecoy(MaskAttributes baseMask, int changes)
    {
        MaskAttributes mask = baseMask;
        int remaining = Mathf.Clamp(changes, 1, 2);

        if (remaining > 0)
        {
            mask.Shape = GetDifferentShape(mask.Shape);
            remaining--;
        }
        if (remaining > 0)
        {
            mask.EyeState = GetDifferentEyeState(mask.EyeState);
            remaining--;
        }
        if (remaining > 0)
        {
            mask.Mouth = GetDifferentMouthMood(mask.Mouth);
        }

        return mask;
    }


    private static MaskAttributes NormalizeMask(MaskAttributes mask, int attributeCount)
    {
        if (attributeCount < 2)
        {
            mask.EyeState = EyeState.None;
            mask.Mouth = MouthMood.None;
        }
        return mask;
    }

    private static int GetMaxUniqueMasks(int attributeCount)
    {
        int shapeCount = 3;
        int eyeCount = attributeCount >= 2 ? 3 : 1;
        int mouthCount = attributeCount >= 2 ? 3 : 1;
        return shapeCount * eyeCount * mouthCount;
    }


    private static MaskShape GetDifferentShape(MaskShape current)
    {
        MaskShape next = GetRandomShape();
        while (next == current)
        {
            next = GetRandomShape();
        }
        return next;
    }

    private static EyeState GetDifferentEyeState(EyeState current)
    {
        EyeState next = GetRandomEyeState();
        while (next == current)
        {
            next = GetRandomEyeState();
        }
        return next;
    }

    private static MouthMood GetDifferentMouthMood(MouthMood current)
    {
        MouthMood next = GetRandomMouthMood();
        while (next == current)
        {
            next = GetRandomMouthMood();
        }
        return next;
    }

    private static MaskShape GetRandomShape()
    {
        return (MaskShape)Random.Range(0, 3);
    }

    private static EyeState GetRandomEyeState()
    {
        return (EyeState)Random.Range(0, 3);
    }

    private static MouthMood GetRandomMouthMood()
    {
        return (MouthMood)Random.Range(0, 3);
    }
}
