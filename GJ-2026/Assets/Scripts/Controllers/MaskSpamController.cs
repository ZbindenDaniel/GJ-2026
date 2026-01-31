using System;
using System.Collections.Generic;
using UnityEngine;
public class MaskSpamController : MonoBehaviour
{
    // Static tuning knobs (fast jam tweaks)
    public static int MinMasks = 3;
    public static int MaxMasks = 8;
    public static float AutoHideSeconds = 20f;

    [Header("World")]
    [SerializeField] private Transform maskParent;
    [SerializeField] private GameObject maskPrefab;
    [SerializeField] private MaskModelLibrary maskModelLibrary;

    [Header("Model Codes")]
    [SerializeField] private EyeColorCode[] eyeColorCodes;
    [SerializeField] private FitTypeCode[] fitTypeCodes;

    [Header("Layout")]
    [SerializeField] private float spacing = 0.2f;
    [SerializeField] private float primaryY = 2.3f;
    [SerializeField] private float x = 0.09f;

    [SerializeField] private float primaryZStart = 0.5f;
    [SerializeField] private float primaryZEnd = -0.5f;
    [SerializeField] private float secondaryZ = 0.5f;
    [SerializeField] private float secondaryYStart = 2.3f;
    [SerializeField] private float secondaryYEnd = 0.6f;
    [SerializeField] private float zDirection = 1f;

    [Header("Testing")]
    [SerializeField] private bool testingMaskCycle = false;
    [SerializeField] private float testingMaskIntervalSeconds = 10f;
    [SerializeField] private bool logSpawnDetails = true;

    private readonly List<GameObject> spawnedMasks = new List<GameObject>();
    private float hideTimer;
    private float testingTimer;
    private LevelDesignData lastDesign;

    public void SpawnMasks(LevelDesignData design)
    {
        SpawnMasksInternal(design, maskParent);
    }

    public void SpawnMasks(LevelDesignData design, Transform worldParent)
    {
        SpawnMasksInternal(design, worldParent);
    }

    private void Update()
    {
        if (hideTimer > 0f)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                ClearMasks();
            }
        }

        if (!testingMaskCycle)
        {
            return;
        }

        testingTimer += Time.deltaTime;
        if (testingTimer >= testingMaskIntervalSeconds)
        {
            testingTimer = 0f;
            if (spawnedMasks.Count > 0)
            {
                ClearMasks();
            }
            else if (maskParent != null && maskPrefab != null && lastDesign != null)
            {
                SpawnMasks(lastDesign);
            }
        }
    }

    public void ClearMasks()
    {
        for (int i = 0; i < spawnedMasks.Count; i++)
        {
            if (spawnedMasks[i] != null)
            {
                Destroy(spawnedMasks[i]);
            }
        }
        spawnedMasks.Clear();
    }

    private List<Vector3> GeneratePositions(int count)
    {
        List<Vector3> positions = new List<Vector3>(count);
        if (count <= 0)
        {
            return positions;
        }

        float primaryRange = Mathf.Abs(primaryZStart - primaryZEnd);
        float direction = primaryZStart >= primaryZEnd ? -1f : 1f;
        float start = primaryZStart;

        float usableRange = Mathf.Max(0f, primaryRange - 2f * spacing);
        int maxPrimary = Mathf.Max(1, Mathf.FloorToInt(usableRange / Mathf.Max(0.0001f, spacing)) + 1);
        int primaryCount = Mathf.Min(count, maxPrimary);
        float primaryStep = primaryCount > 1 ? usableRange / (primaryCount - 1) : 0f;
        if (primaryStep < spacing && primaryCount > 1)
        {
            primaryStep = spacing;
            primaryCount = Mathf.Max(1, Mathf.FloorToInt(usableRange / primaryStep) + 1);
        }

        for (int i = 0; i < primaryCount && positions.Count < count; i++)
        {
            float z = start + direction * (spacing + primaryStep * i);
            positions.Add(new Vector3(x, primaryY, z * zDirection));
        }

        float y = secondaryYStart;
        while (y >= secondaryYEnd - 0.0001f && positions.Count < count)
        {
            positions.Add(new Vector3(x, y, secondaryZ * zDirection));
            y -= spacing;
        }

        return positions;
    }

    private void SpawnMasksInternal(LevelDesignData design, Transform parentTransform)
    {
        if (design == null)
        {
            Debug.LogWarning("MaskSpamController: Missing level design data.");
            return;
        }
        if (maskPrefab == null)
        {
            Debug.LogWarning("MaskSpamController: No mask prefab assigned.");
            return;
        }
        if (parentTransform == null)
        {
            Debug.LogWarning("MaskSpamController: No parent transform assigned.");
            return;
        }

        ClearMasks();

        lastDesign = design;
        List<MaskOptionData> options = design.AvailableMasks ?? new List<MaskOptionData>();
        int count = Mathf.Clamp(options.Count, MinMasks, MaxMasks);
        if (logSpawnDetails)
        {
            Debug.Log($"MaskSpamController spawning {count} masks (options: {options.Count}).");
            Debug.Log($"MaskSpamController parent: {parentTransform.name}");
            Debug.Log($"MaskSpamController parent world pos: {parentTransform.position}, scale: {parentTransform.lossyScale}");
        }

        List<Vector3> positions = GeneratePositions(count);
        if (positions.Count < count)
        {
            Debug.LogWarning($"MaskSpamController generated {positions.Count} positions for {count} masks. Extra masks will use the last position.");
        }
        for (int i = 0; i < count; i++)
        {
            MaskOptionData option = i < options.Count ? options[i] : null;
            GameObject prefabToSpawn = maskPrefab;
            string maskCode = option != null ? BuildMaskCode(option) : null;
            if (maskModelLibrary != null && !string.IsNullOrWhiteSpace(maskCode))
            {
                GameObject resolved = maskModelLibrary.GetPrefab(maskCode);
                if (resolved != null)
                {
                    prefabToSpawn = resolved;
                }
            }

            GameObject maskObject = Instantiate(prefabToSpawn, parentTransform);
            spawnedMasks.Add(maskObject);

            Vector3 position = positions.Count > 0 ? positions[Mathf.Min(i, positions.Count - 1)] : Vector3.zero;
            maskObject.transform.localPosition = position;
            if (logSpawnDetails)
            {
                Debug.Log($"Mask {i} code: {maskCode ?? "none"}, local pos: {maskObject.transform.localPosition}, world pos: {maskObject.transform.position}");
            }

            if (option != null)
            {
                ApplyMaskData(maskObject, option);
                if (logSpawnDetails)
                {
                    Debug.Log($"Mask {i}: shape={option.Mask.Shape}, eye={option.Mask.EyeColor}, pattern={option.Mask.Pattern}, fit={option.FitType}");
                }
            }
        }

        hideTimer = AutoHideSeconds;
    }
    private static void ApplyMaskData(GameObject maskObject, MaskOptionData option)
    {
        if (maskObject == null)
        {
            return;
        }

        MaskSelectable selectable = maskObject.GetComponent<MaskSelectable>();
        if (selectable == null)
        {
            selectable = maskObject.AddComponent<MaskSelectable>();
        }

        selectable.SetData(option);
    }

    [Serializable]
    private struct EyeColorCode
    {
        public EyeColor eyeColor;
        public string code;
    }

    [Serializable]
    private struct FitTypeCode
    {
        public MaskFitType fitType;
        public string code;
    }

    private string BuildMaskCode(MaskOptionData option)
    {
        if (option == null)
        {
            return null;
        }

        string shapeCode = GetShapeCode(option.Mask.Shape);
        string eyeCode = GetEyeCode(option.Mask.EyeColor);
        if (string.IsNullOrWhiteSpace(shapeCode))
        {
            return null;
        }

        if (string.Equals(eyeCode, "O", System.StringComparison.OrdinalIgnoreCase))
        {
            return $"M{shapeCode}.O";
        }

        string moodCode = GetFitCode(option.FitType);
        if (string.IsNullOrWhiteSpace(eyeCode) || string.IsNullOrWhiteSpace(moodCode))
        {
            return null;
        }

        return $"M{shapeCode}.{eyeCode}{moodCode}";
    }

    private static string GetShapeCode(MaskShape shape)
    {
        switch (shape)
        {
            case MaskShape.Round:
                return "R";
            case MaskShape.Square:
                return "S";
            case MaskShape.Triangle:
                return "T";
            default:
                return string.Empty;
        }
    }

    private string GetEyeCode(EyeColor eyeColor)
    {
        if (eyeColor == EyeColor.None)
        {
            return "O";
        }

        if (eyeColorCodes == null)
        {
            return string.Empty;
        }

        for (int i = 0; i < eyeColorCodes.Length; i++)
        {
            if (eyeColorCodes[i].eyeColor == eyeColor)
            {
                return eyeColorCodes[i].code;
            }
        }

        return string.Empty;
    }

    private string GetFitCode(MaskFitType fitType)
    {
        if (fitTypeCodes == null)
        {
            return string.Empty;
        }

        for (int i = 0; i < fitTypeCodes.Length; i++)
        {
            if (fitTypeCodes[i].fitType == fitType)
            {
                return fitTypeCodes[i].code;
            }
        }

        return string.Empty;
    }
}
