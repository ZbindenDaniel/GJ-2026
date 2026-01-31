using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskSpamController : MonoBehaviour
{
    // Static tuning knobs (fast jam tweaks)
    public static int MinMasks = 3;
    public static int MaxMasks = 8;
    public static float AutoHideSeconds = 5f;

    [Header("UI")]
    [SerializeField] private RectTransform maskParent;
    [SerializeField] private GameObject maskPrefab;

    [Header("Layout")]
    [SerializeField] private float padding = 24f;
    [SerializeField] private float spacing = 16f;

    [Header("Testing")]
    [SerializeField] private bool testingMaskCycle = false;
    [SerializeField] private float testingMaskIntervalSeconds = 10f;

    private readonly List<GameObject> spawnedMasks = new List<GameObject>();
    private float hideTimer;
    private float testingTimer;
    private LevelDesignData lastDesign;

    public void SpawnMasks(LevelDesignData design)
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
        if (maskParent == null)
        {
            Debug.LogWarning("MaskSpamController: No mask parent assigned.");
            return;
        }

        ClearMasks();

        lastDesign = design;
        List<MaskOptionData> options = design.AvailableMasks ?? new List<MaskOptionData>();
        int count = Mathf.Clamp(options.Count, MinMasks, MaxMasks);

        List<Vector2> positions = GeneratePositions(count, maskParent.rect);
        for (int i = 0; i < count; i++)
        {
            GameObject maskObject = Instantiate(maskPrefab, maskParent);
            spawnedMasks.Add(maskObject);

            RectTransform rectTransform = maskObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = positions[i];
            }

            if (i < options.Count)
            {
                ApplyMaskData(maskObject, options[i]);
            }
        }

        hideTimer = AutoHideSeconds;
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

    private List<Vector2> GeneratePositions(int count, Rect rect)
    {
        List<Vector2> positions = new List<Vector2>(count);
        float left = rect.xMin + padding;
        float right = rect.xMax - padding;
        float top = rect.yMax - padding;
        float bottom = rect.yMin + padding;

        float width = right - left;
        int maxTopSlots = Mathf.Max(1, Mathf.FloorToInt(width / Mathf.Max(1f, spacing)) + 1);
        int topCount = Mathf.Max(1, Mathf.Min(count, maxTopSlots));
        float topSpacing = topCount > 1 ? width / (topCount - 1) : 0f;

        for (int i = 0; i < count && i < topCount; i++)
        {
            float x = left + topSpacing * i;
            positions.Add(new Vector2(x, top));
        }

        int remaining = count - positions.Count;
        if (remaining <= 0)
        {
            return positions;
        }

        int maxSideSlots = Mathf.Max(1, Mathf.FloorToInt(Mathf.Max(0f, top - bottom) / Mathf.Max(1f, spacing)) + 1);
        int sideSlots = Mathf.Min(Mathf.CeilToInt(remaining / 2f), maxSideSlots);
        float height = Mathf.Max(0f, top - bottom);
        float sideSpacing = sideSlots > 1 ? height / (sideSlots - 1) : 0f;

        for (int i = 0; i < remaining; i++)
        {
            float y = top - sideSpacing * (i / 2f);
            bool leftSide = i % 2 == 0;
            float x = leftSide ? left : right;
            positions.Add(new Vector2(x, y));
        }

        return positions;
    }

    private static void ApplyMaskData(GameObject maskObject, MaskOptionData option)
    {
        Image image = maskObject.GetComponent<Image>();
        if (image != null)
        {
            switch (option.FitType)
            {
                case MaskFitType.Best:
                    image.color = new Color(0.2f, 0.9f, 0.2f, 1f);
                    break;
                case MaskFitType.Partial:
                    image.color = new Color(0.9f, 0.8f, 0.2f, 1f);
                    break;
                default:
                    image.color = new Color(0.9f, 0.2f, 0.2f, 1f);
                    break;
            }
        }
    }
}
