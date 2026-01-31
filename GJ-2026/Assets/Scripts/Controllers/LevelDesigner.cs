using System.Collections.Generic;
using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    // Static tuning knobs (fast jam tweaks)
    public static int MinNpcs = 3;
    public static int MaxNpcs = 12;
    public static int NpcsPerLevel = 1;
    public static int BaseLevel = 1;

    public LevelDesignData GetLevelDesign(int level)
    {
        int safeLevel = Mathf.Max(1, level);
        int attributeCount = GetAttributeCount(safeLevel);
        int npcCount = CalculateNpcCount(safeLevel);

        LevelDesignData design = new LevelDesignData
        {
            LevelIndex = safeLevel,
            NpcCount = npcCount,
            AttributeCount = attributeCount,
            Npcs = new List<NpcDesignData>(npcCount)
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

    private static MaskAttributes CreateMaskAttributes(int attributeCount)
    {
        MaskAttributes mask = new MaskAttributes
        {
            Shape = GetRandomShape(),
            EyeColor = GetRandomEyeColor(),
            Pattern = GetRandomPattern()
        };

        // Reduce attributes for early levels.
        if (attributeCount < 3)
        {
            mask.Pattern = MaskPattern.None;
        }
        if (attributeCount < 2)
        {
            mask.EyeColor = EyeColor.None;
        }

        return mask;
    }

    private static MaskShape GetRandomShape()
    {
        return (MaskShape)Random.Range(0, 3);
    }

    private static EyeColor GetRandomEyeColor()
    {
        return (EyeColor)Random.Range(0, 3);
    }

    private static MaskPattern GetRandomPattern()
    {
        return (MaskPattern)Random.Range(0, 3);
    }
}
