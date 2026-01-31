using UnityEngine;

public class MaskSelectable : MonoBehaviour
{
    [SerializeField] private GameObject highlightObject;
    [SerializeField] private Renderer[] highlightRenderers;
    [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.2f, 1f);

    private Color[] originalColors;
    private bool isSelected;

    public MaskAttributes MaskAttributes { get; private set; }
    public MaskFitType FitType { get; private set; }

    private void Awake()
    {
        if (highlightObject == null)
        {
            highlightObject = FindHighlightChild(transform);
        }

        if ((highlightRenderers == null || highlightRenderers.Length == 0) && highlightObject != null)
        {
            highlightRenderers = highlightObject.GetComponentsInChildren<Renderer>(true);
        }

        if (highlightRenderers != null && highlightRenderers.Length > 0)
        {
            originalColors = new Color[highlightRenderers.Length];
            for (int i = 0; i < highlightRenderers.Length; i++)
            {
                originalColors[i] = highlightRenderers[i] != null ? highlightRenderers[i].material.color : Color.white;
            }
        }

        if (highlightObject != null)
        {
            highlightObject.SetActive(false);
        }
    }

    public void SetData(MaskOptionData option)
    {
        if (option == null)
        {
            return;
        }

        MaskAttributes = option.Mask;
        FitType = option.FitType;
    }

    public void SetHighlighted(bool active)
    {
        if (isSelected)
        {
            return;
        }

        if (highlightObject != null)
        {
            highlightObject.SetActive(active);
        }
        else if (active)
        {
            Debug.LogWarning($"MaskSelectable on {name} has no highlightObject assigned or found.");
        }

        if (highlightRenderers != null && highlightRenderers.Length > 0)
        {
            for (int i = 0; i < highlightRenderers.Length; i++)
            {
                Renderer renderer = highlightRenderers[i];
                if (renderer == null)
                {
                    continue;
                }

                renderer.enabled = active;
                if (active)
                {
                    renderer.material.color = highlightColor;
                }
                else if (originalColors != null && i < originalColors.Length)
                {
                    renderer.material.color = originalColors[i];
                }
            }
        }
    }

    public bool Select()
    {
        SetHighlighted(false);

        // Get the parent NPCControl and call 'EvaluateMask'
        NpcControl npcControl = GetComponentInParent<NpcControl>();
        if (npcControl != null)
        {
            Debug.Log($"MaskSelectable: {name} selected mask for NPC {npcControl.name}.");
            npcControl.EvaluateMask();
        }

        return true;
    }

    private static GameObject FindHighlightChild(Transform root)
    {
        if (root == null)
        {
            return null;
        }

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child == null)
            {
                continue;
            }

            string name = child.name.ToLowerInvariant();
            if (name.Contains("highlight") || name.Contains("flare") || name.Contains("outline"))
            {
                return child.gameObject;
            }
        }

        return null;
    }
}
