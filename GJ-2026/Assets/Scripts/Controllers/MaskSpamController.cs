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
    [SerializeField] private float circleRadius = 2.5f;
    [SerializeField] private float maskYOffset = 3f;

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
        if (logSpawnDetails)
        {
            Debug.Log($"MaskSpamController spawning {count} masks (options: {options.Count}).");
        }

        List<Vector3> positions = GeneratePositions(count);
        for (int i = 0; i < count; i++)
        {
            GameObject maskObject = Instantiate(maskPrefab, maskParent);
            spawnedMasks.Add(maskObject);

            RectTransform rectTransform = maskObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(positions[i].x, positions[i].y);
            }
            else
            {
                if (i == 0)
                {
                    Debug.LogWarning("MaskSpamController: maskPrefab has no RectTransform. Use a UI prefab (Image/Button) under a Canvas.");
                }
                maskObject.transform.localPosition = positions[i];
            }

            if (i < options.Count)
            {
                ApplyMaskData(maskObject, options[i]);
                if (logSpawnDetails)
                {
                    Debug.Log($"Mask {i}: shape={options[i].Mask.Shape}, eye={options[i].Mask.EyeColor}, pattern={options[i].Mask.Pattern}, fit={options[i].FitType}");
                }
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

    private List<Vector3> GeneratePositions(int count)
    {
        List<Vector3> positions = new List<Vector3>(count);
        if (count <= 0)
        {
            return positions;
        }

        float step = 360f / count;
        float startAngle = 0f;

        for (int i = 0; i < count; i++)
        {
            float offsetIndex = i == 0 ? 0f : Mathf.Ceil(i / 2f);
            float direction = (i % 2 == 0) ? -1f : 1f;
            float angle = startAngle + direction * step * offsetIndex;
            float radians = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radians) * circleRadius;
            float z = -Mathf.Cos(radians) * circleRadius;
            positions.Add(new Vector3(x, maskYOffset, z));
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
