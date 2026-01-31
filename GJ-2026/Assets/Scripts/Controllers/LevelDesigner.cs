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
            AvailableMasks = new List<MaskOptionData>(),
            Elevators = new List<ElevatorDesignData>(),
            PlayerElevatorIndex = 0
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

        design.AvailableMasks = CreateMaskOptions(design.Npcs, attributeCount, safeLevel);
        design.Elevators = CreateElevators();
        design.PlayerElevatorIndex = GetPlayerElevatorIndex(design.Elevators.Count);
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
        if (level <= 4)
        {
            return 2;
        }
        return 3;
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

        // Reduce attributes for early levels.
        if (attributeCount < 3)
        {
            mask.Mouth = MouthMood.None;
        }
        if (attributeCount < 2)
        {
            mask.EyeState = EyeState.None;
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

    private static List<MaskOptionData> CreateMaskOptions(List<NpcDesignData> npcs, int attributeCount, int level)
    {
        int maskCount = CalculateMaskCount(level);
        int maxUnique = GetMaxUniqueMasks(attributeCount);
        maskCount = Mathf.Min(maskCount, maxUnique);
        List<MaskOptionData> options = new List<MaskOptionData>(maskCount);

        if (npcs == null || npcs.Count == 0)
        {
            for (int i = 0; i < maskCount; i++)
            {
                options.Add(new MaskOptionData { Mask = CreateMaskAttributes(attributeCount), FitType = MaskFitType.None });
            }
            return options;
        }

        MaskAttributes mostCommon = GetMostCommonMask(npcs, attributeCount);
        HashSet<MaskAttributes> used = new HashSet<MaskAttributes> { mostCommon };

        options.Add(new MaskOptionData { Mask = mostCommon, FitType = MaskFitType.Best });

        int guard = 0;
        while (options.Count < maskCount && guard < 200)
        {
            MaskOptionData next = CreateNextMaskOption(mostCommon, attributeCount, options);
            if (used.Add(next.Mask))
            {
                options.Add(next);
            }
            guard++;
        }

        return options;
    }

    private static MaskOptionData CreateNextMaskOption(MaskAttributes baseMask, int attributeCount, List<MaskOptionData> existing)
    {
        bool needPartial = !HasFitType(existing, MaskFitType.Partial);
        bool needNone = !HasFitType(existing, MaskFitType.None);

        if (needPartial)
        {
            return new MaskOptionData { Mask = CreatePartialMask(baseMask, attributeCount), FitType = MaskFitType.Partial };
        }

        if (needNone)
        {
            return new MaskOptionData { Mask = CreateNoneMask(baseMask, attributeCount), FitType = MaskFitType.None };
        }

        // Mix of partial and none once we already have both.
        return Random.Range(0, 2) == 0
            ? new MaskOptionData { Mask = CreatePartialMask(baseMask, attributeCount), FitType = MaskFitType.Partial }
            : new MaskOptionData { Mask = CreateNoneMask(baseMask, attributeCount), FitType = MaskFitType.None };
    }

    private static bool HasFitType(List<MaskOptionData> options, MaskFitType type)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].FitType == type)
            {
                return true;
            }
        }
        return false;
    }

    private static MaskAttributes GetMostCommonMask(List<NpcDesignData> npcs, int attributeCount)
    {
        Dictionary<MaskAttributes, int> counts = new Dictionary<MaskAttributes, int>();
        MaskAttributes best = npcs[0].Mask;
        int bestCount = 0;

        for (int i = 0; i < npcs.Count; i++)
        {
            MaskAttributes mask = NormalizeMask(npcs[i].Mask, attributeCount);
            if (!counts.ContainsKey(mask))
            {
                counts[mask] = 0;
            }

            counts[mask]++;
            if (counts[mask] > bestCount)
            {
                bestCount = counts[mask];
                best = mask;
            }
        }

        return best;
    }

    private static MaskAttributes NormalizeMask(MaskAttributes mask, int attributeCount)
    {
        if (attributeCount < 3)
        {
            mask.Mouth = MouthMood.None;
        }
        if (attributeCount < 2)
        {
            mask.EyeState = EyeState.None;
        }
        return mask;
    }

    private static int GetMaxUniqueMasks(int attributeCount)
    {
        int shapeCount = 3;
        int eyeCount = attributeCount >= 2 ? 3 : 1;
        int patternCount = attributeCount >= 3 ? 3 : 1;
        return shapeCount * eyeCount * patternCount;
    }

    private static MaskAttributes CreatePartialMask(MaskAttributes baseMask, int attributeCount)
    {
        MaskAttributes mask = baseMask;

        if (attributeCount >= 3)
        {
            mask.Mouth = GetDifferentMouthMood(baseMask.Mouth);
        }
        else if (attributeCount == 2)
        {
            mask.EyeState = GetDifferentEyeState(baseMask.EyeState);
        }
        else
        {
            mask.Shape = GetDifferentShape(baseMask.Shape);
        }

        return mask;
    }

    private static MaskAttributes CreateNoneMask(MaskAttributes baseMask, int attributeCount)
    {
        MaskAttributes mask = baseMask;

        mask.Shape = GetDifferentShape(baseMask.Shape);
        if (attributeCount >= 2)
        {
            mask.EyeState = GetDifferentEyeState(baseMask.EyeState);
        }
        if (attributeCount >= 3)
        {
            mask.Mouth = GetDifferentMouthMood(baseMask.Mouth);
        }

        return mask;
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
