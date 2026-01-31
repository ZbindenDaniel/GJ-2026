using UnityEngine;

public class MaskSelectable : MonoBehaviour
{
    [SerializeField] private GameObject highlightObject;
    [SerializeField] private Renderer[] highlightRenderers;
    [SerializeField] private SpriteRenderer highlightSprite;
    [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.2f, 1f);

    private Color[] originalColors;
    private bool isSelected;

    public MaskAttributes MaskAttributes { get; private set; }
    public MaskFitType FitType { get; private set; }

    private void Awake()
    {
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
        if (highlightSprite != null)
        {
            highlightSprite.enabled = false;
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
        if (highlightSprite != null)
        {
            highlightSprite.enabled = active;
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
        if (isSelected)
        {
            return false;
        }

        isSelected = true;
        SetHighlighted(false);
        gameObject.SetActive(false);
        return true;
    }
}
